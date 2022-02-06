let index = 0;

function AddTag(){
    var tagEntry = document.getElementById("TagEntry");

    let searchResult = searchTags(tagEntry.value);
    if (searchResult != null) {
        // trigger sweetAlert
        Swal.fire({
            title: 'Error!',
            text: searchResult,
            icon: 'error',
            confirmButtonText: 'Continue'
        });
    } else {
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("tagList").options[index++] = newOption;
    }

    tagEntry.value = "";
    return true;
}

function DeleteTag() {
    let tagCount = 1;
    var tagList = document.getElementById('tagList');
    if (!tagList) return false;
    if (tagList.selectedIndex == -1) {
        Swal.fire({
            title: 'Error!',
            text: "Please choose a tag before deleting.",
            icon: 'error',
            confirmButtonText: 'Continue'
        });
        return true;
    }
    while (tagCount > 0) {
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

// search function will detect either an empty or a duplicate tag
// returns error string if error is detected

function searchTags(str) {
    if (str == "") {
        return "Empty tags are not permitted.";
    }
    var tagsElement = document.getElementById('tagList');
    if (tagsElement) {
        let options = tagsElement.options;
        for (let i = 0; i < options.length; i++) {
            if (options[i].value == str) {
                return `The duplicate tag: #${str} will not be added.`;
            }
        }
    }
}
