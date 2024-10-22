const showHiddenPass = (loginPass, loginEye) => {
    const input = document.getElementById(loginPass),
        iconEye = document.getElementById(loginEye)

    iconEye.addEventListener('click', () => {
        // Change password to text
        if (input.type === 'password') {
            // Switch to text
            input.type = 'text'

            // Icon change
            iconEye.classList.add('fa-eye')
            iconEye.classList.remove('fa-eye-slash')
        } else {
            // Change to password
            input.type = 'password'

            // Icon change
            iconEye.classList.remove('fa-eye')
            iconEye.classList.add('fa-eye-slash')
        }
    })
}

showHiddenPass('login-pass', 'password-eye')
showHiddenPass('signup-pass', 'password-eye')
showHiddenPass('signup-confirm-pass', 'confirm-password-eye')

var password = document.getElementById("signup-pass")
    , confirm_password = document.getElementById("signup-confirm-pass");

function validatePassword() {
    if (password.value != confirm_password.value) {
        confirm_password.setCustomValidity("Mật khẩu không trùng nhau!");
    } else {
        confirm_password.setCustomValidity('');
    }
}

password.onchange = validatePassword;
confirm_password.onkeyup = validatePassword;