document.addEventListener("DOMContentLoaded", () => {
    // Kiểm tra nếu chưa có widget thì mới load
    if (!document.getElementById("chat-widget")) {
        fetch('/chat/chatwidget')
            .then(res => res.text())
            .then(html => {
                const wrapper = document.createElement('div');
                wrapper.innerHTML = html;
                document.body.appendChild(wrapper);
            });
    }
});
