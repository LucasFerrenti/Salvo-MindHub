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
        $("#logout-btn").hide();
        this.getGames();
    },
    methods: {
        renderGlobalTable() {
            this.playerTable = "";
            this.getGames()
        },
        renderPlayerTable() {
            this.playerTable = "true";
            this.getGames()
        },
        back() {
            window.location.href = '/index.html';
        },
        registerPage() {
            window.location.href = '/register.html';
        },
        getGames: function () {
            this.showLogin(false);
            axios.get('/api/games')
                .then(response => {
                    this.player = response.data.email;
                    this.games = response.data.games;
                    this.historyGames = this.getHistoryGames(this.games);
                    if (this.player == "Guest") {
                        this.showLogin(true);
                        this.playerTable = "";
                    }
                    else {
                        $("#logout-btn").show();
                    }
                })
                .catch(error => {
                    console.log(error.data);
                    this.modal.tittle = "Error " + error.status;
                    this.modal.message = error.data;
                    this.showModal(true);
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
                    console.log(error.response.data);
                    this.modal.tittle = "Error " + error.reponse.status;
                    this.modal.message = error.response.data;
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
                        this.getGames();
                    }
                })
                .catch(error => {
                    console.log("error, código de estatus: " + error.response.status);
                    if (error.response.status == 401) {
                        this.modal.tittle = "Fallo de autenticacion";
                        this.modal.message = error.response.data;
                        this.showModal(true);
                    }
                    else {
                        this.modal.tittle = "Error " + error.response.status;
                        this.modal.message = error.response.data;
                        this.showModal(true);
                    }
                });
        },
        getHistoryGames: function (games) {
            let historyGames = [];
            games.forEach(game => {
                let historyGame = { gameId: 0, player1: "", player2: "", status: "", creationDate: "" };
                historyGame.gameId = game.id;
                historyGame.player1 = game.gamePlayers[0]?.player.email;
                if (game.gamePlayers.length >= 2) {
                    historyGame.player2 = game.gamePlayers[1]?.player.email;
                }
                else {
                    historyGame.player2 = "Esperando oponente";
                }
                switch (game.gamePlayers[0]?.point) {
                    case 0:
                        historyGame.status = "Gano " + historyGame.player2;
                        break;
                    case 1:
                        historyGame.status = "Gano " + historyGame.player1;
                        break;
                    case 0.5:
                        historyGame.status = "Empate";
                        break;
                    default:
                        historyGame.status = "En Progreso";
                }
                historyGame.creationDate = game.creationDate;
                historyGames.push(historyGame);
            });
            return historyGames.reverse();
        }
    },
    filters: {
        dateFormat(stringDate) {
            let date = new Date(stringDate)
            let utcDiff = date.getTimezoneOffset() / 60;
            let localDate = new Date(date.setHours(date.getHours() - utcDiff)).toJSON();
            return moment(date).format('LLL');
        }
    }
});