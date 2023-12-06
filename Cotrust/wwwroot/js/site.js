
/*DINAMICA DE LAS TARJETAS DE LOS PRODUCTOS*/
function cardmouseover(id) {
    div = document.getElementById(id)
    div.className = "card shadow h-100"
}
function cardmouseout(id) {
    div = document.getElementById(id)
    div.className = "card shadow h-100 border-4"
}

/*DINAMICA DEL MENU DE HOME*/
function menumouseover(id) {
    item = document.getElementById(id)
    item.className = "navbar-brand fw-bold"
}
function menumouseout(id) {
    item = document.getElementById(id)
    item.className = "navbar-brand"
}

/*DINAMICA DEL MENU DEL LAYOUT*/
function layoutmouseover(id) {
    item = document.getElementById(id)
    item.className = "nav-link active fw-bold"
}
function layoutmouseout(id) {
    item = document.getElementById(id)
    item.className = "nav-link active"
}