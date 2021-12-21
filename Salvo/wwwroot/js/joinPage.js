const urlParams = new URLSearchParams(window.location.search);
const gameID = urlParams.get('game');

var app = new Vue({
    el: "#app",
    data: {
        email:"",
        password:"",
        player:"",
        gameJoin: 0,
        modal: {
            tittle: "",
            message: ""
        }
    },
    async mounted(){
        this.showLogin(false);
        axios.get("/api/auth/getauth")
            .then(result => {
                this.player = result.data;
                if (this.player == "Guest") {
                    this.showLogin(true);
                }
                else {
                    $("#logout-btn").show();
                }
                this.joinGame(gameID);
            })
            .catch(error => {
                console.error(error);
                this.modal.tittle = "Error";
                this.modal.message = error.data;
                this.showModal(true);
            });
    },
    methods: {
        joinGame(gId) {
            var gpId = null;
            axios.post("/api/games/" + gId + "/players")
                .then(response => {
                    if (response.data[0] == "<"){
                        this.modal.tittle = "Error";
                        this.modal.message = "Inicie sesion para unirse";
                        this.showModal(true);
                        return;
                    }
                    gpId = response.data;
                    window.location.href = "/game.html?gp=" + gpId;
                })
                .catch(error => {
                    console.error(error);
                    this.modal.tittle = "Error " + error.response.status;
                    this.modal.message = error.response.data;
                    this.showModal(true);
                });
        },
        logout: function () {
            axios.post('/api/auth/logout')
                .then(result => {
                    if (result.status == 200) {
                        this.showLogin(true);
                        location.reload();
                    }
                })
                .catch(error => {
                    console.error(error);
                    this.modal.tittle = "Error " + error.response.status;
                    this.modal.message = error.response.data;
                    this.injectHTML = "";
                    this.showModal(true);
                });
        },
        login: function (event) {
            axios.post('/api/auth/login', {
                email: this.email, password: this.password, user: ""
            })
                .then(result => {
                    if (result.status == 200) {
                        this.showLogin(false);
                        location.reload();
                    }
                })
                .catch(error => {
                    console.error(error);
                    if (error.response.status == 401) {
                        this.modal.tittle = "Fallo de autenticacion";
                        this.modal.message = error.response.data;
                        this.injectHTML = "";
                        this.showModal(true);
                    }
                    else if(error.response.status == 403){
                        this.modal.tittle = "Usuario sin activar";
                        this.modal.message = "";
                        this.injectHTML= "<i> 'Verfique su correo electronico o carpeta de spam para activarlo, caso contrario reenvie el correo haciendo click '</i><a href='/index.html?register=resend/" + this.email + "'>aqui</a>";
                        this.showModal(true);
                    }
                    else {
                        this.modal.tittle = "error";
                        this.injectHTML = "";
                        this.modal.message = error.response.data;
                        this.showModal(true);
                    }
                });
        },
        showLogin: function (show) {
            if (show) {
                $("#login-form").show();
                $("#login-form").trigger("reset");
                this.email = "";
                this.password = "";
                this.user = "";
                $("#logout-btn").hide();
            }
            else
                $("#login-form").hide();
        },
        showModal: function (show) {
            if (show)
                $("#infoModal").modal('show');
            else
                $("#infoModal").modal('hide');
        },
        registerPage() {
            window.location.href = '/register.html';
        }
    }
})