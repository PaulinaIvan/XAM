﻿const username = localStorage.getItem('currentUsername');

if (username === null || username === '' || username === undefined) {
    showLoginScreen();
} else {
    fetch(`/Home/CheckIfExpired?username=${username}`)
        .then(response => {
            if (response.ok) {
                console.log(`Logged in as ${username}.`);
                showMainPage();
            } else {
                console.log('Session expired.');
                showLoginScreen();
            }
        });
}

document.getElementById('loginButton').addEventListener('click', () => {
    const inputUsername = document.getElementById('usernameField').value.trim();

    if (inputUsername === '') {
        alert('Please enter a username.');
    } else {
        loginFunction(inputUsername);
    }
});

document.getElementById('logoutButton').addEventListener('click', logoutReset);

function loginFunction(inputUsername) {
    fetch(`/Home/UsernameLogin?username=${inputUsername}`)
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Failed to login.');
            }
        })
        .then(() => {
            console.log(`Logged in as ${inputUsername}.`);
            localStorage.setItem('currentUsername', inputUsername);
            showMainPage();
        })
        .catch(error => {
            console.error('Error:', error);
            showLoginScreen();
        });
}

function showLoginScreen() {
    document.getElementById('loginScreen').style.display = 'block';
    document.getElementById('mainPage').style.display = 'none';
}

function showMainPage() {
    document.getElementById('loginScreen').style.display = 'none';
    document.getElementById('mainPage').style.display = 'block';
}

function logoutReset() {
    localStorage.removeItem('currentUsername');
    showLoginScreen();
    document.getElementById('usernameField').value = '';
}
