

// Create a "close" button and append it to each list item
function CreateCloseButton() {
    var myNodelist = document.getElementsByTagName("LI");
    var i;
    for (i = 0; i < myNodelist.length; i++) {
        // alert(myNodelist[i].textContent);
        var span = document.createElement("SPAN");
        var txt = document.createTextNode("\u00D7");
        span.className = "close";
        span.appendChild(txt);
        myNodelist[i].isPressed = "false";
        myNodelist[i].appendChild(span);
    }

}


// Click on a close button to hide the current list item
function CloseButtonClicked() {
    var close = document.getElementsByClassName("close");
    var i;
    for (i = 0; i < close.length; i++) {
        close[i].onclick = function () {
            var div = this.parentElement;
            if (div.isPressed == "false") {
                div.style.display = "none";
            }
            else {
                div.isPressed = "false";
                div.classList.toggle('checked');
            }
        }
    }
}

// Add a "checked" symbol when clicking on a list item
function AddCheckedSymbol() {
    var list = document.querySelector('ul');
    list.addEventListener('click', function (ev) {
        var checkList = document.getElementsByTagName("LI");
        var i;
        // Check if there is a "checked" symbol
        for (i = 0; i < checkList.length; i++) {
            if (checkList[i].isPressed == "true") {
                checkList[i].isPressed = "false";
                checkList[i].classList.toggle('checked');
            }
        }
        // Add a "checked" symbol to the current item
        if (ev.target.tagName === 'LI' ) {
            if (ev.target.isPressed == "false") {
                ev.target.isPressed = "true";

            }
            else
                ev.target.isPressed = "false";
            ev.target.classList.toggle('checked');

        }
    }, false);
}

// Create a new list item when clicking on the "Add" button ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
function newElement() {
    var li = document.createElement("li");
    var inputValue = document.getElementById("myInput").value;
    var t = document.createTextNode(inputValue);
    li.appendChild(t);
    window.alert(inputValue);
    //listBinding.add(inputValue);
    if (inputValue === '') {
        alert("You must write something!");
    } else {
        document.getElementById("myUL").appendChild(li);

    }
    var span = document.createElement("SPAN");
    var txt = document.createTextNode("\u00D7");
    span.className = "close";
    span.appendChild(txt);
    li.appendChild(span);
    for (i = 0; i < close.length; i++) {
        close[i].onclick = function () {
            var div = this.parentElement;
            div.style.display = "none";
        }
    }
}

function Run() {
    CreateCloseButton();
    CloseButtonClicked();
    AddCheckedSymbol();

}
