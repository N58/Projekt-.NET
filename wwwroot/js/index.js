let tabs = ["recipes-tab", "users-tab", "categories-tab"]
let pages = ["recipes", "users", "categories"]

function addOnclick() {
    for (let i = 0; i < tabs.length; i++) {
        document.getElementById(tabs[i]).onclick = function () {
            localStorage.setItem("activeTab", i)
        }
    }
}

function loadTab() {
    let pick = localStorage.getItem("activeTab")
    if (pick === null) {
        pick = 0;
    }
    for (let i = 0; i < tabs.length; i++) {
        if (pick == i) {
            document.getElementById(tabs[i]).setAttribute("class", "nav-link active")
            document.getElementById(tabs[i]).setAttribute("aria-selected", "true")
            document.getElementById(pages[i]).setAttribute("class", "tab-pane fade show active")
        }
        else {
            document.getElementById(tabs[i]).setAttribute("class", "nav-link")
            document.getElementById(tabs[i]).setAttribute("aria-selected", "false")
            document.getElementById(pages[i]).setAttribute("class", "tab-pane fade")
        }
    }
}


addOnclick()
loadTab()