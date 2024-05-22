var commentButton = document.getElementById("writeComment");
var commentForm = document.getElementById("newComment");
commentButton.addEventListener("click", function () {
   commentButton.setAttribute("style","display:none");
   commentForm.setAttribute("style","");
})