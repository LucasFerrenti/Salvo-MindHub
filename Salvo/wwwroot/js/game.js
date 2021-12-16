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
        interval: null
    },
    mounted() {
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
            if(this.interval == null && this.player != "Guest"){
                this.interval = setInterval(this.getGames, 5000);
            }
            else if (this.player != "Guest"){
                this.$forceUpdate();
            }
            else{
                clearInterval(this.interval);
            }
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
                    this.modal.tittle = "Error " + error.response.status;
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
                        this.modal.tittle = "error";
                        this.modal.message = error.response.data;
                        this.showModal(true);
                    }
                });
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
        getMyGames: function (games){
            let myGames = [];
            games.forEach(game =>{
                game.gamePlayers.forEach(gp =>{
                    if (gp.player.email == this.player)
                        myGames.push(game); 
                });
            });
            return myGames;
        },
        getJoineableGames: function (games){
            let joineableGames = [];
            games.forEach(game =>{
                if(game.gamePlayers.length == 1 && game.gamePlayers[0].player?.email != this.player){
                    joineableGames.push(game);
                }
            });
            return joineableGames;
        }
    },
    filters: {
        dateFormat(date) {
            let time = moment(date).format('l') + moment(date).format(' LTS');
            return time;
        }
    }
})

