const urlParams = new URLSearchParams(window.location.search);
const register = urlParams.get('register');

var app = new Vue({
    el: '#app',
    data: {
        games: [],
        openGames: [],
        myGames: [],
        joineableGames: [],
        scores: [],
        email: "",
        password: "",
        user: "",
        modal: {
            tittle: "",
            message: ""
        },
        player: null,
        show: false,
        interval: null,
        injectHTML: ""
    },
    mounted() {
        $("#logout-btn").hide();
        if( register == null){
            this.getGames();
            return
        }
        let data = register.split("/")
        history.pushState(null, "", "index.html")
        if (data[0] == "successed"){
            this.modal.tittle = "Registro exitoso"
            this.modal.message = "Verifique su correo electronico para activar su cuenta"
            this.injectHTML = "";
            this.showModal(true);
        }
        else if (data[0] == "resend"){
            if (data.length < 2){
                this.modal.tittle = "Datos invalidos"
                this.modal.message = "Verifique el formato"
                this.injectHTML = "";
                this.showModal(true);
            }
            else{
                this.resendMail(data[1]);
            }
        }
        this.getGames();
    },
    methods: {
        historyPage() {
            window.location.href = '/history.html';
        },
        registerPage() {
            window.location.href = '/register.html';
        },
        joinGame(gId) {
            var gpId = null;
            axios.post('/api/games/' + gId + '/players')
                .then(response => {
                    gpId = response.data;
                    window.location.href = '/game.html?gp=' + gpId;
                })
                .catch(error => {
                    this.modal.tittle = "Error " + error.response.status;
                    this.modal.message = error.response.data;
                    this.injectHTML = "";
                    this.showModal(true);
                });
        },
        createGame() {
            var gpId = null;
            axios.post('/api/games')
                .then(response => {
                    gpId = response.data;
                    window.location.href = '/game.html?gp=' + gpId;
                })
                .catch(error => {
                    this.modal.tittle = "Error " + error.response.status;
                    this.modal.message = error.response.data;
                    this.injectHTML = "";
                    this.showModal(true);
                });
        },
        returnGame(gpId) {
            window.location.href = '/game.html?gp=' + gpId;
        },
        getGames: async function () {
            console.log("hola");
            this.showLogin(false);
            await axios.get('/api/games')
                .then(response => {
                    this.player = response.data.email;
                    this.games = response.data.games;
                })
                .catch(error => {
                    console.log(error.data);
                    this.modal.tittle = "Error " + error.status;
                    this.modal.message = error.data;
                    this.injectHTML = "";
                    this.showModal(true);
                });
            this.openGames = this.getOpenGames(this.games);
            this.myGames = this.getMyGames(this.openGames);
            this.joineableGames = this.getJoineableGames(this.openGames);
            this.getScores(this.games);
            if (this.player == "Guest") {
                this.showLogin(true);
            }
            else {
                $("#logout-btn").show();
            }
            if (this.interval == null && this.player != "Guest") {
                this.interval = setInterval(this.getGames, 5000);
            }
            else if (this.player != "Guest") {
                this.$forceUpdate();
            }
            else {
                clearInterval(this.interval);
            }
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
                        this.getGames();
                    }
                })
                .catch(error => {
                    console.log("error, código de estatus: " + error.response.status);
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
        resendMail(email){
            this.showModal(false);
            this.showLoadingModal(true);
            this.delay(500);
            axios.post("/api/players/resend/" + email)
                .then(result => {
                    this.showLoadingModal(false);
                    this.modal.tittle = "Reenvio exitoso";
                    this.modal.message = "Verifique nuevamente su correo";
                    this.injectHTML = "";
                    this.showModal(true);
                })
                .catch(error => {
                    this.modal.tittle = "Error en el Reenvio";
                    this.modal.message = error.response.data;
                    this.injectHTML = "";
                    this.showModal(true);
                })
        },
        getScores: function (games) {
            var scores = [];
            games.forEach(game => {
                game.gamePlayers.forEach(gp => {
                    var index = scores.findIndex(sc => sc.email == gp.player.email)
                    if (index < 0) {
                        var score = { email: gp.player.email, win: 0, tie: 0, lost: 0, total: 0 }
                        switch (gp.point) {
                            case 1:
                                score.win++;
                                break;
                            case 0:
                                score.lost++;
                                break;
                            case 0.5:
                                score.tie++;
                                break;
                        }
                        score.total += gp.point;
                        scores.push(score);
                    }
                    else {
                        switch (gp.point) {
                            case 1:
                                scores[index].win++;
                                break;
                            case 0:
                                scores[index].lost++;
                                break;
                            case 0.5:
                                scores[index].tie++;
                                break;
                        }
                        scores[index].total += gp.point;
                    }
                })
            })
            app.scores = scores;
        },
        getOpenGames: function (games) {
            let openGames = [];
            games.forEach(game => {
                if (game.gamePlayers[0]?.point == null) {
                    openGames.push(game);
                }
            })
            return openGames;
        },
        getMyGames: function (games) {
            let myGames = [];
            games.forEach(game => {
                game.gamePlayers.forEach(gp => {
                    if (gp.player.email == this.player)
                        myGames.push(game);
                });
            });
            return myGames;
        },
        getJoineableGames: function (games) {
            let joineableGames = [];
            games.forEach(game => {
                if (game.gamePlayers.length == 1 && game.gamePlayers[0].player?.email != this.player) {
                    joineableGames.push(game);
                }
            });
            return joineableGames;
        },
        delay(ms){
            return new Promise(function(resolve){
            setTimeout(resolve,ms);
            });
        }
    },
    filters: {
        dateFormat(stringDate) {
            let date = new Date(stringDate)
            let utcDiff = date.getTimezoneOffset() / 60;
            let localDate = new Date(date.setHours(date.getHours() - utcDiff)).toJSON();
            return moment(localDate).format('l') + moment(localDate).format(' LTS');
        }
    }
})

