
var app = new Vue({
    el: '#app',
    data: {
        email: "",
        password: "",
        user: "",
        modal: {
            tittle: "",
            message: ""
        },
        player: "",
        emailError: "",
        userError: "",
        passwordError: ""
    },
    mounted() {
        axios.get("/api/auth/getauth")
            .then(result => {
                this.player = result.data;
                if (this.player != "Guest") {
                    this.back();
                }
            })
            .catch(error => {
                console.error(error);
                this.modal.tittle = "error";
                this.modal.message = error.data;
                this.showModal(true);
            });
    },
    methods: {
        validEmail(email) {
            if (email == "") {
                return { text: "Email Vacio", isValid: false };
            }
            let emailPattern = /^([\w\.\-]+)@([\w\-]+)((\.\w{2,3})+)$/;
            if (!emailPattern.test(email)) {
                return { text: "Email invalido", isValid: false };
            }
            //checked
            return { text: "", isValid: true };
        },
        validUser(user) {
            //check min length
            if (user.length < 4) {
                return { text: "Usuario invalido: Debe contener al menos 4 caracteres", isValid: false };
            }
            //check max length
            if (user.length > 16) {
                return { text: "Usuario invalido: No debe contener mas de 16 caracteres", isValid: false };
            }
            //checked
            return { text: "", isValid: true };
        },
        validPassword(password) {
            //check min length
            if (password.length < 8) {
                return { text: "Contraceña invalida: Debe contener al menos 8 caracteres", isValid: false };
            }
            //check max length
            if (password.length > 100) {
                return { text: "Contraceña invalida: No debe contener mas de 100 caracteres", isValid: false };
            }
            //check upercase
            let upercasePattern = /(?=.*[A-Z])./;
            if (!upercasePattern.test(password)) {
                return { text: "Contraceña invalida: Debe contener al menos una letra mayuscula", isValid: false };
            }
            //check lowercase
            let lowercasePattern = /(?=.*[a-z])./;
            if (!lowercasePattern.test(password)) {
                return { text: "Contraceña invalida: Debe contener al menos una letra minuscula", isValid: false };
            }
            //check number or special characters
            let numSpecPattern = /((?=.*\d)|(?=.*\W+))./;
            if (!numSpecPattern.test(password)) {
                return { text: "Contraseña Invalida: Debe contener al menos un numero o caracter especial", isValid: false }
            }
            //check new line
            let newLinePattern = /(?=.*\n)./
            if (newLinePattern.test(password)) {
                return { text: "Contraseña Invalida: No puede contener saltos de linea", isValid: false };
            }
            //checked
            return { text: "", isValid: true };
        },
        async checkEmailForm() {
            //check email
            let email = this.validEmail(this.email);
            let emailForm = $("#email-form")
            this.emailError = email.text;
            if (email.isValid)
                emailForm.removeClass("is-invalid").addClass("is-valid");
            else
                emailForm.removeClass("is-valid").addClass("is-invalid");
            //check is available
            if (emailForm.hasClass("is-invalid"))
                return
            let result;
            await axios.get("/api/players/available/" + "" + this.email)
                .then(res => {
                    result = res.data;
                })
                .catch(error => {
                    console.error(error);
                    this.modal.tittle = "error";
                    this.modal.message = error.data;
                    this.showModal(true);
                });
            if (result == true) {
                emailForm.removeClass("is-invalid").addClass("is-valid");
                this.emailError = "";
            }
            else {
                emailForm.removeClass("is-valid").addClass("is-invalid");
                this.emailError = "Email en uso";
            }
        },
        checkUserForm() {
            //check user
            let user = this.validUser(this.user);
            let userForm = $("#user-form")
            this.userError = user.text;
            if (user.isValid)
                userForm.removeClass("is-invalid").addClass("is-valid");
            else
                userForm.removeClass("is-valid").addClass("is-invalid");
        },
        checkPasswordForm() {
            //check password
            let password = this.validPassword(this.password);
            let passwordForm = $("#password-form")
            this.passwordError = password.text;
            if (password.isValid == true)
                passwordForm.removeClass("is-invalid").addClass("is-valid");
            else
                passwordForm.removeClass("is-valid").addClass("is-invalid");
        },
        async checkRegisterForm(){
            //check is successful
            let valids = $(".is-valid");
            console.log(valids.length);
            if (valids.length < 3)
                return;
            $("#signin-btn").attr("disabled", "disabled")
            $("#cancel-btn").attr("disabled", "disabled")
            valids.each(function () {
                $(this).removeClass("is-valid");
                $(this).attr("disabled", "disabled")
            });
            this.showLoadingModal(true);
            await this.delay(500)

            await this.signin();
            
            this.showLoadingModal(false);
            $("#signin-btn").removeAttr("disabled");
            $("#cancel-btn").removeAttr("disabled");
            valids.each(function () {
                $(this).removeAttr("disabled");
                $(this).val("");
            });
        },
        back() {
            window.location.href = '/index.html';
        },
        showModal: function (show) {
            if (show)
                $("#infoModal").modal('show');
            else
                $("#infoModal").modal('hide');
        },
        showLoadingModal(show){
            if (show)
                $("#loadingModal").modal('show');
            else
                $("#loadingModal").modal('hide');
        },
        signin: async function (event) {
            await axios.post('/api/players', {
                email: this.email, password: this.password, user: this.user
            })
                .then(result =>{
                    window.location.href = '/index.html?register=successed'
                })
                .catch(error => {
                    console.error(error);
                    this.modal.tittle = "Error";
                    this.modal.message = error.response.data;
                    this.showModal(true);
                });
        },
        delay(ms){
            return new Promise(function(resolve){
            setTimeout(resolve,ms);
            });
        }
    },
});

