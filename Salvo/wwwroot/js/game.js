var app = new Vue({
    el: '#app',
    data: {
        games: [],
        scores: []
    },
    mounted() {
        axios.get('/api/games')
            .then(response => {
                this.games = response.data;
                getScores(this.games)
            })
            .catch(error => {
                alert("erro al obtener los datos");
            })
    },
    filters: {
        dateFormat(date) {
            return moment(date).format('LLL');
        }
    }
})

function getScores(games) {
    var scores = [];
    games.forEach(game => {
        game.gamePlayers.forEach(gp => {
            var index = scores.findIndex(sc => sc.email == gp.player.email)
            if (index < 0) {
                var score = { email: gp.player.email, win: 0, tie: 0, lost: 0, total : 0 }
                switch (gp.point){
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
}