const checkedItems = []

document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("create-case-submit").addEventListener("click", () => {
        handleSubmit()
    })

    document.querySelectorAll('.case-item').forEach((caseItem) => {
        caseItem.addEventListener("click", (e) => {
            const caseItem = e.currentTarget
            const caseItemId = caseItem.getAttribute("data-item-id")

            caseItem.classList.toggle("checked")

            document.querySelectorAll('.percentage-controls').forEach((controls) => {
                if (controls.getAttribute('data-item-id') == caseItemId) {
                    controls.classList.toggle('d-none')
                }
            })

            const index = checkedItems.findIndex(x => x.itemId == caseItemId)

            if (index === -1) {
                checkedItems.push({
                    itemId: caseItemId,
                    percentageValue: 0 
                })
                changePercentageControlsValue(caseItemId, 0)
                
            } else {
                checkedItems.splice(index, 1)
            }

            console.log(checkedItems)
        })
    })

    document.querySelectorAll('.percentage-slider').forEach((percentageSlider) => {
        percentageSlider.addEventListener("input", (e) => {
            const percentageSlider = e.currentTarget
            const percentageSliderId = percentageSlider.getAttribute("data-item-id")            

            const index = checkedItems.findIndex(x => x.itemId == percentageSliderId)

            if (index !== -1) {
                checkedItems[index].percentageValue = parseInt(percentageSlider.value)
            } else {
                console.error("error while changing percentageSlider")
            }

            updateTotalPercentage()
            changePercentageInputvalue(percentageSliderId, e.currentTarget.value)

            //const sum = checkedItems.reduce((acc, curr) => acc + curr.percentageValue, 0)

            //if (sum > 100) {
            //    e.preventDefault()
            //    checkedItems[index].percentageValue = parseInt(e.target.value)
            //    changePercentageInputvalue(percentageSliderId, e.target.value)
            //    changePercentageSliderValue(percentageSliderId, e.target.value)
            //} else {
            //    changePercentageInputvalue(percentageSliderId, e.currentTarget.value)
            //}
        })

        //percentageSlider.addEventListener("beforeinput", (e) => {
        //    const percentageSlider = e.currentTarget
        //    const percentageSliderId = percentageSlider.getAttribute("data-item-id")

        //    const index = checkedItems.findIndex(x => x.itemId == percentageSliderId)

        //    if (index !== -1) {
        //        checkedItems[index].percentageValue = parseInt(percentageSlider.value)
        //    }

        //    const sum = checkedItems.reduce((acc, curr) => acc + curr.percentageValue, 0)
        //    console.log("spicha")

        //    if (sum > 100) {
        //        console.log("spucha")
        //        e.preventDefault()
        //        checkedItems[index].percentageValue = e.target.value
        //    } else {
        //        changePercentageInputvalue(percentageSliderId, e.target.value)
        //    }
        //})
    })

    document.querySelectorAll('.percentage-input').forEach((percentageInput) => {
        percentageInput.addEventListener("input", (e) => {
            const percentageInput = e.currentTarget
            const percentageInputId = percentageInput.getAttribute("data-item-id")

            const index = checkedItems.findIndex(x => x.itemId == percentageInputId)

            if (index !== -1) {
                checkedItems[index].percentageValue = parseInt(percentageInput.value)
            } else {
                console.error("error while changing percentageInput")
            }

            const sum = checkedItems.reduce((acc, curr) => acc + curr.percentageValue, 0)
            console.log(sum)

            updateTotalPercentage()
            changePercentageSliderValue(percentageInputId, e.currentTarget.value)

            //if (sum > 100) {
            //    e.preventDefault()
            //    checkedItems[index].percentageValue = parseInt(e.target.value)
            //    console.log(e.target.value)
            //    changePercentageInputvalue(percentageInput, e.target.value)
            //    changePercentageControlsValue(percentageInput, e.target.value)
            //} else {
            //    changePercentageSliderValue(percentageInputId, e.currentTarget.value)
            //}

        })

        //percentageInput.addEventListener("beforeinput", (e) => {
        //    const percentageInput = e.currentTarget
        //    const percentageInputId = percentageInput.getAttribute("data-item-id")

        //    const index = checkedItems.findIndex(x => x.itemId == percentageInputId)

        //    console.log(e.target.value)
        //    console.log(e.currentTarget.value)
        //    if (index !== -1) {
        //        checkedItems[index].percentageValue = parseInt(e.currentTarget.value)
        //    }

        //    console.log(checkedItems)
        //    const sum = checkedItems.reduce((acc, curr) => acc + curr.percentageValue, 0)
        //    console.log(sum)

        //    if (sum > 100) {
        //        e.preventDefault()
        //        e.stopPropagation()
        //        checkedItems[index].percentageValue = e.target.value
        //    } else {
        //        changePercentageSliderValue(percentageInputId, e.currentTarget.value)
        //    }
        //})
    })
})

async function handleSubmit() {
    const itemsValidation = document.getElementById("items-validation")
    const nameValidation = document.getElementById("name-validation")
    let errors = 0
    console.log(getTotalPercentage())
    if (getTotalPercentage() !== 100) {
        itemsValidation.innerText = "Sum of selected cases percentage should be 100%"
        errors += 1
    } else {
        itemsValidation.innerText = ""
    }

    const name = document.getElementById("case-name").value

    if (name == "" || name == null || name == undefined) {
        nameValidation.innerText = "Name shouldn't be empty"
        errors += 1
    } else {
        nameValidation.innerText = ""
    }

    if (errors > 0) {
        return
    }

    const antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value
    const formData = new FormData()

    formData.append("__RequestVerificationToken", antiForgeryToken)
    console.log(checkedItems)
    formData.append("Items", checkedItems)
    formData.append("Name", name)

    const payload = {
        antiForgeryToken: antiForgeryToken,
        Items: checkedItems,
        Name: name
    }

    try {
        const response = await fetch(`/Cases/Create`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload),
            credentials: "include",
        })

        if (response.status === 200) {
            window.location.replace(`${document.location.origin}/${await response.text()}`)
        };
    } catch (error) {
        console.log(error)
    }
}

function updateTotalPercentage() {
    const totalPercentageElement = document.getElementById("total-percentage");
    let totalPercentage = 0

    for (let i = 0; i < checkedItems.length; i++) {
        totalPercentage += checkedItems[i].percentageValue
    }
    console.log(totalPercentage)
    totalPercentageElement.innerText = `Total percentage: ${totalPercentage}%`
}

function getTotalPercentage() {
    let total = 0

    for (item of checkedItems) {
        total += item.percentageValue
    }

    return total
}

function changePercentageControlsValue(itemId, value) {
    changePercentageInputvalue(itemId, value)
    changePercentageSliderValue(itemId, value)
}

function changePercentageSliderValue(itemId, value) {
    document.querySelectorAll('.percentage-slider').forEach((percentageSlider) => {
        if (percentageSlider.getAttribute('data-item-id') == itemId) {
            percentageSlider.value = value
        }
    })
}

function changePercentageInputvalue(itemId, value) {
    document.querySelectorAll('.percentage-input').forEach((percentageInput) => {
        if (percentageInput.getAttribute("data-item-id") == itemId) {
            percentageInput.value = value
        }
    })
}