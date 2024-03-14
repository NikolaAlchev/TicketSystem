let flag1 = true;
let flag2 = true;

function showPassword() {
    var x = document.getElementById("Password");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
    if (flag1) {
        document.getElementById("passwordEye").style.display = "none";
        document.getElementById("disableEye1").style.display = "inline-block";
    } else {
        document.getElementById("passwordEye").style.display = "inline-block";
        document.getElementById("disableEye1").style.display = "none";
    }
    flag1 = !flag1;
    
}

function showConfirmPassword() {
    var x = document.getElementById("ConfirmPassword");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
    if (flag2) {
        document.getElementById("confirmPasswordEye").style.display = "none";
        document.getElementById("disableEye2").style.display = "inline-block";
    } else {
        document.getElementById("confirmPasswordEye").style.display = "inline-block";
        document.getElementById("disableEye2").style.display = "none";
    }
    flag2 = !flag2;
}

