var commentButton = document.getElementById("writeComment");
var commentForm = document.getElementById("newComment");
commentButton.addEventListener("click", function () {
   commentButton.setAttribute("style","display:none");
   commentForm.setAttribute("style","");
})
const replyButtons = document.querySelectorAll('.replyButton');
replyButtons.forEach(function (button) {
   button.addEventListener('click', function () {
      const form = this.nextElementSibling;
      this.style.display = 'none';
      form.style.display = 'block';
   });
});
const hideButtons = document.querySelectorAll('.hideButton');
hideButtons.forEach(function (button) {
   button.addEventListener('click', function () {
      const comment = this.parentElement
      const showButton = comment.previousElementSibling.lastElementChild
      showButton.style.display = 'block'
      comment.style.display = 'none';
   });
});
const showButtons = document.querySelectorAll('.showButton');
showButtons.forEach(function (button) {
   button.addEventListener('click', function () {
      const comment = this.parentElement.nextElementSibling;
      this.style.display = 'none';
      comment.style.display = 'block';
   });
});