const selectedToSell = []
let selectedSum = 0
let allSelected = false

document.addEventListener("DOMContentLoaded", () => {
    Array.from(document.getElementsByClassName("item")).forEach(i => i.addEventListener("click", (e) => { handleSelect(e) }))

    document.getElementById("sell-selected").addEventListener("click", handleSubmit)

    document.getElementById("select-all").addEventListener("click", selectAll)
})

function selectAll() {
    const items = Array.from(document.getElementsByClassName("item"))
    const selectAllButton = document.getElementById("select-all")
    selectedToSell.length = 0

    if (!allSelected) {
        selectAllButton.innerText = "Deselect all items"

        items.forEach(i => i.classList.remove("selected"))

        for (let i = 0; i < items.length; i++) {
            handleSelect({
                currentTarget: items[i]
            })
        }

        items.forEach(i => i.classList.add("selected"))
    } else {
        selectedSum = 0
        document.getElementById("selected-sum").innerText = selectedSum
        selectAllButton.innerText = "Select all items"
        items.forEach(i => i.classList.remove("selected"))
    }

    allSelected = !allSelected
}

async function handleSubmit() {
    const sellSelectedButton = document.getElementById("sell-selected")
    const errrorMessageElement = document.getElementById("error-message")
    sellSelectedButton.disabled = true

    errrorMessageElement.classList.add("d-none")
    errrorMessageElement.classList.remove("d-block")
    errrorMessageElement.textContent = ""

    try {
        const response = await fetch("/Cases/SellCase", {
            method: 'POST',
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(selectedToSell),
            credentials: 'include'
        })

        if (response.status === 200) {
            window.location.replace(`${document.location.origin}/User/Inventory`)
        } else {
            errrorMessageElement.classList.add("d-block")
            errrorMessageElement.classList.remove("d-none")
            errrorMessageElement.textContent = "Cannot sell chest"
        }
    } catch (error) {
        errrorMessageElement.classList.add("d-block")
        errrorMessageElement.classList.remove("d-none")
        errrorMessageElement.textContent = "Cannot sell chest"
    } finally {
        sellSelectedButton.disabled = false
    }
}

function handleSelect(e) {
    const id = e.currentTarget.getAttribute("data-item-id")
    const isSelected = e.currentTarget.classList.contains("selected")
    const itemPrice = parseInt(e.currentTarget.textContent.split(" - ")[1])
    const items = selectedToSell.filter(i => i.Id == id)

    console.log(itemPrice)

    if (isSelected) {

        selectedSum -= itemPrice

        if (items.length === 1) {
            items[0].Count = items[0].Count - 1
        }
        e.currentTarget.classList.remove("selected")
    } else {

        selectedSum += itemPrice

        if (items.length === 0) {
            selectedToSell.push({
                Id: id,
                Count: 1
            })
        } else if (items.length === 1) {
            items[0].Count = items[0].Count + 1
        }
        e.currentTarget.classList.add("selected")
    }

    document.getElementById("selected-sum").innerText = selectedSum

    console.log(selectedToSell)
}