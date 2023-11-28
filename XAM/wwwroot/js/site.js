const username = localStorage.getItem('currentUsername');

if (username === null || username === '' || username === undefined) {
    if(location.pathname !== '/')
        window.location.href = '/'
    showLoginScreen();
} else {
    fetch(`/Home/CheckIfExpired?username=${username}`)
        .then(response => response.json())
        .then(data => {
            if(data.errorCode === 'NoSession')
            {
                showLoginScreen();
            }
            else
            {
                console.log(`Logged in as ${username}.`);
                showMainPage();
            }
        })
        .catch(error => {
            console.error('Error:', error);
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
            localStorage.setItem('currentUsername', inputUsername);
            resetToIndex();
        })
        .catch(error => {
            console.error('Error:', error);
            resetToIndex();
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

document.getElementById('logoutButton').addEventListener('click', () => {
    localStorage.removeItem('currentUsername');
    showLoginScreen();
    document.getElementById('usernameField').value = '';
    resetToIndex();
});

function resetToIndex()
{
    window.location.href = '/';
}
