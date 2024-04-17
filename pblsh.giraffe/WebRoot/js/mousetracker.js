const background = document.querySelector(".shiny");
background.addEventListener("mousemove", (e) => {
    const { x, y } = background.getBoundingClientRect();
    background.style.setProperty("--x", e.clientX - x);
    background.style.setProperty("--y", e.clientY - y);
});