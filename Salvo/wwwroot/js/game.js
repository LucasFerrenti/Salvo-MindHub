var app = new Vue({
    el: '#app',
    data: {
        games: [],
    },
    mounted() {
        axios.get('/api/games')
            .then(response => {
                this.games = response.data;
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