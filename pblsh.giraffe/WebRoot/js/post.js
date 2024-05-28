const toggleReplies = document.querySelectorAll(".toggle-replies")
const togglmao = {"none": "", "": "none"}

toggleReplies.forEach((btn) => {
    btn.addEventListener("click", () => {
        const replies = btn.nextElementSibling
        const answerCount = replies.nextElementSibling
        replies.style.display = togglmao[replies.style.display]
        answerCount.style.display = togglmao[answerCount.style.display]
    })
})

const openReplyBox = document.querySelectorAll(".comment-open-reply-box")
const replyBox = document.querySelectorAll(".comment-reply-container")
const closeReplyBox = document.querySelectorAll(".comment-reply-cancel")

openReplyBox.forEach((btn, i) => {
    btn.addEventListener("click", () => {
        replyBox[i].style.display = "flex"
    })
})


closeReplyBox.forEach((btn, i) => {
    btn.addEventListener("click", () => {
        replyBox[i].style.display = "none"
    })
})