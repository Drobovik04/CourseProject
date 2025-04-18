﻿const templateId = Number(new URLSearchParams(window.location.search).get('id'));
const connection = new signalR.HubConnectionBuilder().withUrl("/CommentHub").build();

document.getElementById('add-comment-btn').addEventListener('click', () => {
    const newCommentHtml = `
                <div id="new-comment" class="comment-item p-3 rounded bg-light mb-2">
                    <textarea id="new-comment-text" class="form-control" placeholder=""></textarea>
                    <div class="mt-2 d-flex justify-content-end">
                        <button id="cancel-comment-btn" class="btn btn-secondary me-2">@localizer["Cancel"]</button>
                        <button id="save-comment-btn" class="btn btn-success">@localizer["Save"]</button>
                    </div>
                </div>`;

    const commentsContainer = document.getElementById('comments-container');
    commentsContainer.insertAdjacentHTML('afterbegin', newCommentHtml);

    document.getElementById('cancel-comment-btn').addEventListener('click', () => {
        const newCommentDiv = document.getElementById('new-comment');
        newCommentDiv.remove();
    });

    document.getElementById('save-comment-btn').addEventListener('click', async () => {
        const text = document.getElementById('new-comment-text').value;
        if (!text.trim()) {
            alert('@localizer["CannotBeEmpty"]');
            return;
        }

        const response = await fetch('/Comment/AddComment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ Text: text, TemplateId: templateId })
        });

        if (response.ok) {
            document.getElementById('new-comment').remove();
        } else {
            alert('@localizer["ErrorSaveComment"]');
        }
    });
});

function deleteComment(commentId) {
    if (!confirm('@localizer["AreYouSure?"]')) {
        return;
    }

    fetch(`/Comment/DeleteComment?id=${commentId}`, {
        method: 'DELETE'
    })
    .then(response => {
        if (response.ok) {
            removeCommentFromPage(commentId);
        } else {
            alert('@localizer["ErrorDeleteComment"]');
        }
    })
}

connection.on("ReceiveComment", (comment) => {
    addCommentToPage(comment);
});

connection.on("UpdateComment", (updatedComment) => {
    updateCommentOnPage(updatedComment);
});

connection.on("DeleteComment", (commentId) => {
    removeCommentFromPage(commentId);
});

connection.start().then(() => {
    connection.invoke("JoinTemplateGroup", templateId);
});

function addCommentToPage(comment) {
    const commentsContainer = document.getElementById("comments-container");
    const localTime = formatDateToShort(comment.createdAt).replace(',', '');
    const commentHtml = `
                <div id="comment-${comment.id}" class="comment-item p-3 rounded bg-light mb-2">
                    <div class="d-flex justify-content-between">
                        <strong>${comment.author}</strong>
                        <div>
                            <button class="btn btn-link btn-sm edit-button" 
                                    data-author="${comment.userId}" 
                                    data-comment-id="${comment.id}" 
                                    style="display: none;" 
                                    onclick="editComment(${comment.id})">@localizer["Edit"]</button>
                            <button class="btn btn-link btn-sm text-danger delete-button" 
                                    data-author="${comment.userId}" 
                                    data-comment-id="${comment.id}" 
                                    style="display: none;" 
                                    onclick="deleteComment(${comment.id})">@localizer["Delete"]</button>
                        </div>
                    </div>
                    <div class="text-muted small">${localTime}</div>
                    <div class="mt-2">${comment.text}</div>
                </div>`;
    commentsContainer.insertAdjacentHTML("afterbegin", commentHtml);
    const newCommentDiv = document.getElementById(`comment-${comment.id}`);
    if (isAdmin || comment.userId === currentUserId) {
        newCommentDiv.querySelectorAll(`button[data-comment-id="${comment.id}"]`).forEach(button => {
            button.style.display = "inline-block";
        });
    }
}

function formatDateToShort(dateString) {
    const date = new Date(dateString);

    const options = {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
    };

    return new Intl.DateTimeFormat('ru-RU', options).format(date);
}
function updateCommentOnPage(updatedComment) {
    const commentDiv = document.getElementById(`comment-${updatedComment.id}`);
    if (commentDiv) {
        commentDiv.querySelector(".mt-2").textContent = updatedComment.text;
    }
}

function removeCommentFromPage(commentId) {
    const commentDiv = document.getElementById(`comment-${commentId}`);
    if (commentDiv) {
        commentDiv.remove();
    }
}

function editComment(commentId) {
    const commentDiv = document.getElementById(`comment-${commentId}`);
    const commentText = commentDiv.querySelector('.mt-2').textContent.trim();

    commentDiv.querySelector('.mt-2').innerHTML = `
        <textarea id="edit-comment-text-${commentId}" class="form-control">${commentText}</textarea>
        <div class="mt-2 d-flex justify-content-end">
            <button class="btn btn-secondary me-2" onclick="cancelEditComment(${commentId}, '${commentText}')">@localizer["Cancel"]</button>
            <button class="btn btn-success" onclick="saveComment(${commentId})">@localizer["Save"]</button>
        </div>
    `;
}

function cancelEditComment(commentId, originalText) {
    const commentDiv = document.getElementById(`comment-${commentId}`);
    commentDiv.querySelector('.mt-2').textContent = originalText; 
}

function saveComment(commentId) {
    const newText = document.getElementById(`edit-comment-text-${commentId}`).value.trim();

    if (!newText) {
        alert('@localizer["CannotBeEmpty"]');
        return;
    }

    fetch(`/Comment/UpdateComment?id=${commentId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ text: newText })
    });
}
