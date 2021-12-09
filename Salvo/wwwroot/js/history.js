var app = new Vue({
    el: '#app',
    data: {
        games: [],
        historyGames: [],
        playerTable: "",
        email: "",
        password: "",
        user: "",
        modal: {
            tittle: "",
            message: ""
        },
        player: ""
    },
    mounted() {
        this.getGames();
    },
    methods: {
        renderGlobalTable(){
            this.playerTable = "";
            this.getGames()
        },
        renderPlayerTable(){
            this.playerTable = "true";
            this.getGames()
        },
        back(){
            window.location.href = '/index.html';
        },
        getGames: function (){
            this.showLogin(false);
            axios.get('/api/games')
                .then(response => {
                    this.player = response.data.email;
                    this.games = response.data.games;
                    this.historyGames = this.getHistoryGames(this.games);
                    if (this.player == "Guest"){
                        this.showLogin(true);
                        this.playerTable = "";
                    }
                    else{
                        $("#logout-btn").show();
                    }
                })
                .catch(error => {
                    alert(error);
                });
        },
        showModal: function (show) {
            if (show)
                $("#infoModal").modal('show');
            else
                $("#infoModal").modal('hide');
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
        logout: function () {
            axios.post('/api/auth/logout')
                .then(result => {
                    if (result.status == 200) {
                        this.showLogin(true);
                        this.getGames();
                    }
                })
                .catch(error => {
                    alert("Ocurrió un error al cerrar sesión");
                });
        },
        login: function(event){
            axios.post('/api/auth/login', {
                email: this.email, password: this.password, user: this.user
            })
                .then(result => {
                    if (result.status == 200) {
                        this.showLogin(false);
                        this.getGames();
                    }
                })
                .catch(error => {
                    console.log("error, código de estatus: " + error.response.status);
                    if (error.response.status == 401) {
                        this.modal.tittle = "Falló la autenticación";
                        this.modal.message = "Email o contraseña inválido"
                        this.showModal(true);
                    }
                    else {
                        this.modal.tittle = "Fall&Oacute;la autenticaci&oacute;n";
                        this.modal.message = "Ha ocurrido un error";
                        this.showModal(true);
                    }
                });
        },
        signin: function (event) {
            axios.post('/api/players', {
                email: this.email, password: this.password, user: this.user
            })
                .then(result => {
                    if (result.status == 201) {
                        this.login();
                    }
                })
                .catch(error => {
                    console.log("error, código de estatus: " + error.response.status);
                    if (error.response.status == 403) {
                        this.modal.tittle = "Falló el registro";
                        this.modal.message = error.response.data
                        this.showModal(true);
                    }
                    else {
                        this.modal.tittle = "Fall&Oacute;la autenticaci&oacute;n";
                        this.modal.message = "Ha ocurrido un error";
                        this.showModal(true);
                    }
                });
        },
        getHistoryGames: function(games){
            let historyGames = [];
            games.forEach(game => {
                let historyGame = {gameId: 0, player1: "", player2: "", status: "", creationDate:""};
                historyGame.gameId = game.id;
                historyGame.player1 = game.gamePlayers[0].player.email;
                if(game.gamePlayers.length >= 2){
                    historyGame.player2 = game.gamePlayers[1].player.email;
                }
                else{
                    historyGame.player2 = "Esperando oponente";
                }
                switch (game.gamePlayers[0].point){
                    case 0:
                        historyGame.status = "Gano " + historyGame.player2 ;
                        break;
                    case 1:
                        historyGame.status = "Gano " + historyGame.player1;
                        break;
                    case 0.5:
                        historyGame.status = "Empate";
                        break;
                    case null:
                        historyGame.status = "En Progreso";
                }
                historyGame.creationDate = game.creationDate;
                historyGames.push(historyGame);
            });
            return historyGames.reverse();
        }
    },
    filters: {
        dateFormat(date) {
            return moment(date).format('LLL');
        }
    }
})

