var app = new Vue({
    el: '#app',
    data: {
        games: [],
        openGames: [],
        scores: [],
        email: "",
        password: "",
        user: "",
        modal: {
            tittle: "",
            message: ""
        },
        player: null
    },
    mounted() {
        this.getGames();
    },
    methods: {
        historyPage(){
            window.location.href = '/history.html';
        },
        registerPage(){
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
                    alert("erro al unirse al juego");
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
                    alert("error al obtener los datos");
                });
        },
        returnGame(gpId) {
            window.location.href = '/game.html?gp=' + gpId;
        },
        getGames: function (){
            this.showLogin(false);
            axios.get('/api/games')
                .then(response => {
                    this.player = response.data.email;
                    this.games = response.data.games;
                    this.openGames = this.getOpenGames(this.games);
                    this.getScores(this.games)
                    if (this.player == "Guest"){
                        this.showLogin(true);
                    }
                    else{
                        $("#logout-btn").show();
                    }
                })
                .catch(error => {
                    alert("erro al obtener los datos");
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
        getOpenGames: function(games){
            let openGames = [];
            games.forEach(game =>{
                if(game.gamePlayers[0].point == null){
                    openGames.push(game);
                }
            })
            return openGames;
        }
    },
    filters: {
        dateFormat(date) {
            return moment(date).format('LLL');
        }
    }
})

