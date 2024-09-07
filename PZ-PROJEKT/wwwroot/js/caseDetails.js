const ERROR_INSUFFICIENT_BALANCE = "Insufficient balance to open case."
const ERROR_USER_NOT_LOGGED_IN = "User not authenticated."
const ERROR_UNKNOWN = "Error while opening the case."
const SPINNER_ITEM_PADDING_PIXELS = 8
const NUM_SPINNER_ITEMS = 11

document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("spin-button").addEventListener("click", async () => {
        const spinButtonElement = document.getElementById("spin-button")
        spinButtonElement.disabled = true
        
        const result = await openCase();

        const errorMessageElement = document.getElementById("error-message")

        if (result.error) {
            if (result.error === ERROR_USER_NOT_LOGGED_IN) {
                window.location.replace(`${document.location.origin}/Auth`)
            } else if(result.error === ERROR_INSUFFICIENT_BALANCE) {
                errorMessageElement.textContent = result.error
                errorMessageElement.classList.remove("d-none")
                errorMessageElement.classList.add("d-block")
            }
            spinButtonElement.disabled = false
            return
        } else {
            errorMessageElement.classList.remove("d-block")
            errorMessageElement.classList.add("d-none")
        }

        const droppedItemNameElement = document.getElementById("opened-item-name")

        droppedItemNameElement.classList.remove("d-block")
        droppedItemNameElement.classList.add("d-none")

        let droppedItemText = `${result[0].Skin.Name} - ${result[0].Skin.Name}`
        const skinPrice = parseInt(result[0].Skin.Price)
        const casePrice = parseInt(document.getElementById("case-price").textContent)

        if (skinPrice > casePrice) {
            droppedItemText += " <span class='w'>"
        } else {
            droppedItemText += " <span class='l'>"
        }

        droppedItemText += skinPrice > casePrice ? `+${skinPrice - casePrice}` : `${skinPrice - casePrice}`

        droppedItemNameElement.innerHTML = droppedItemText

        if (parseInt(result[0].Id) !== -1) {
            const caseItems = Array.from(document.getElementById("items").children)
            let numSpins = 0;

            if (caseItems.length > NUM_SPINNER_ITEMS) {
                numSpins = caseItems.length * 3
            } else {
                numSpins = NUM_SPINNER_ITEMS * 3
            }

            spin(1, numSpins, parseInt(result[0].Id))
        }
    })

    window.addEventListener("resize", resizeWinningLine)

    resizeWinningLine()
})

function spin(spinNumber, totalSpins, result) {
    const spinnerItemsElement = document.getElementById("spinner-items")
    const spinnerItems = Array.from(spinnerItemsElement.children).filter(elem => !elem.classList.contains("spinner-helper"))
    const caseItems = Array.from(document.getElementById("items").children)

    const width = spinnerItems[0].children[0].width + SPINNER_ITEM_PADDING_PIXELS
    const tmp = spinnerItems[5].getAttribute("data-item-id")

    //console.log(`${result} - ${tmp}`)
    //console.log(`${spinNumber / totalSpins}s`)
    //spinnerItems.forEach(function (part, index, theArray) {
    //    theArray[index].style.transitionDuration = `${(Math.round(100 * spinNumber / totalSpins) / 10).toFixed(2)}s`
    //});

    if (spinNumber >= totalSpins && spinnerItems[5].getAttribute("data-item-id") == result) {
        document.getElementById("spin-button").disabled = false

        const droppedItemNameElement = document.getElementById("opened-item-name")

        droppedItemNameElement.classList.remove("d-none")
        droppedItemNameElement.classList.add("d-block")

        return
    }

    spinnerItems[0].addEventListener("transitionend", (e) => {
        const spinnerItemsElement = document.getElementById("spinner-items")
        const spinnerItems = Array.from(spinnerItemsElement.children).filter(elem => !elem.classList.contains("spinner-helper"))

        spinnerItems[0].classList.remove("spinner-item")
        spinnerItems[0].style.transform = ""
        spinnerItemsElement.removeChild(spinnerItems[0])

        const spinnerItem = createSpinnerItem(caseItems[(spinNumber + 10) % caseItems.length], parseInt(width) * spinNumber)
        spinnerItemsElement.insertBefore(spinnerItem, document.getElementById("spinner-helper-right"))

        for (let i = 0; i < spinnerItems.length; i++) {
            spinnerItems[i].classList.remove("spinner-item");
            spinnerItems[i].style.transform = "";
        }

        setTimeout(() => {
            /*spinnerItems[4].classList.remove("winning-line")
            spinnerItems[5].classList.add("winning-line")*/

            spin(spinNumber + 1, totalSpins, result)
        }, 1)
    })

    for (let j = 0; j < spinnerItems.length; j++) {
        spinnerItems[j].classList.add("spinner-item");
        spinnerItems[j].classList.add(spinNumber);
        spinnerItems[j].style.transform = `translate(-${parseInt(width)}px)`
    }

}

async function openCase() {
    const caseId = document.getElementById("case-id").value

    if (!caseId) {
        console.error("Cannot find chest id")
        return
    }

    try {
        const antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value
        const formData = new FormData()

        formData.append("__RequestVerificationToken", antiForgeryToken)

        const response = await fetch(`/Cases/Open/${caseId}?itemsToRoll=1`, {
            method: "POST",
            body: formData,
            credentials: "include",
        })
        
        if (response.status === 401) {
            const error = await response.text()

            return {
                error: error
            }
        }

        const responseBody = await JSON.parse(await response.json())

        return responseBody
    } catch (error) {
        console.log("wtf")
        return {
            error: ERROR_UNKNOWN
        }
    }

    return {
        skinId: -1,
        skinName: null
    }
}

function resizeWinningLine() {
    const winningLine = document.getElementById("winning-line")
    const imgs = Array.from(document.getElementsByClassName("spinner-item-img"))

    if (imgs.length > 0) {
        winningLine.style.height = `${imgs[0].height + SPINNER_ITEM_PADDING_PIXELS + 1}px`
    }
}

function createSpinnerItem(fromItem, width) {
    const spinnerItem = document.createElement("div")

    spinnerItem.classList.add("col")
    spinnerItem.classList.add("spinner-item")

    const spinnerItemImg = document.createElement("img")

    spinnerItemImg.src = fromItem.getAttribute("data-img-src")
    spinnerItem.setAttribute("data-item-id", fromItem.getAttribute("data-item-id"))
    spinnerItemImg.classList.add("img-thumbnail")
    spinnerItem.style.transform = `translate(-${parseInt(width)})`

    spinnerItem.appendChild(spinnerItemImg)

    return spinnerItem
}