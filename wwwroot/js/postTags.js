let index = 0;

function AddTag(){
    var tagEntry = document.getElementById("TagEntry");
    let newOption = new Option(tagEntry.value, tagEntry.value);
    document.getElementById("tagList").options[index++] = newOption;

    tagEntry.value = "";
    return true;
}

function DeleteTag() {
    let tagCount = 1;
    while (tagCount > 0) {
        const tagList = document.getElementById("tagList");
        let selectedIndex = tagList.selectedIndex;
        if (selectedIndex >= 0) {
            tagList.options[selectedIndex] = null;
            --tagCount;
        } else {
            tagCount = 0;
        }
        index--;
    }
}

$("form").on("submit", function () {
    $("#tagList option").prop("selected", "selected");
})

if (tagValues !== "") {
    let tagArr = tagValues.split(",");
    for (var loop = 0; loop < tagArr.length; loop++) {
        replaceTag(tagArr[loop], loop);
        index++;
    }
}

function replaceTag(tag, idx) {
    let newOption = new Option(tag, tag);
    const tagList = document.getElementById("tagList");
    tagList.options[idx] = newOption;
}