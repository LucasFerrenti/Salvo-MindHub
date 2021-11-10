const urlParams = new URLSearchParams(window.location.search);
const gpId = urlParams.get('gp');

var app = new Vue({
    el: '#app',
    data: {
        gameView: {},
        player: { email: null },
        opponent: { email: null }
    },
    mounted() {
        axios.get('/api/gamePlayers/'+gpId)
            .then(response => {
                this.gameView = response.data;
                getPlayers(this.gameView, gpId);
                initializeGrid(this.gameView);
                placeShips(this.gameView.ships);
                placeSalvos(this.gameView.salvos, this.player.id, this.gameView.ships)
                addEvents();
            })
            .catch(error => {
                alert("erro al obtener los datos");
            })
    }
})

function getPlayers(gameView,gpId) {
    gameView.gamePlayers.forEach(gp => {
        if (gp.id == gpId)
            app.player = gp.player;
        else
            app.opponent = gp.player;
    });
}

function initializeGrid(gameview) {
    var options = {
        //grilla de 10 x 10
        width: 10,
        height: 10,
        //separacion entre elementos (les llaman widgets)
        verticalMargin: 0,
        //altura de las celdas
        cellHeight: 40,
        //desabilitando el resize de los widgets
        disableResize: true,
        //widgets flotantes
        float: true,
        //removeTimeout: 100,
        //permite que el widget ocupe mas de una columna
        disableOneColumnMode: true,
        //false permite mover, true impide
        staticGrid: false,
        //activa animaciones (cuando se suelta el elemento se ve más suave la caida)
        animate: true
    }
    //se inicializa el grid con las opciones
    $('.grid-stack').gridstack(options);
}

function placeShips(ships) {
    grid = $('#grid').data('gridstack');
    ships = JSON.parse(JSON.stringify(ships));
    ships.forEach(ship => {
        ship.locations.sort((a, b) => {
            if (a.location > b.location)
                return 1;
            else if (a.location < b.location)
                return -1;
            else
                return 0;
        });

        var searchChar = ship.locations[0].location.slice(0, 1);
        var secondChar = ship.locations[1].location.slice(0, 1);
        if (searchChar === secondChar) {
            ship.position = "Horizontal";
        } else {
            ship.position = "Vertical";
        }
        for (var i = 0; i < ship.locations.length; i++) {
            ship.locations[i].location = ship.locations[i].location.replace(/A/g, '0');
            ship.locations[i].location = ship.locations[i].location.replace(/B/g, '1');
            ship.locations[i].location = ship.locations[i].location.replace(/C/g, '2');
            ship.locations[i].location = ship.locations[i].location.replace(/D/g, '3');
            ship.locations[i].location = ship.locations[i].location.replace(/E/g, '4');
            ship.locations[i].location = ship.locations[i].location.replace(/F/g, '5');
            ship.locations[i].location = ship.locations[i].location.replace(/G/g, '6');
            ship.locations[i].location = ship.locations[i].location.replace(/H/g, '7');
            ship.locations[i].location = ship.locations[i].location.replace(/I/g, '8');
            ship.locations[i].location = ship.locations[i].location.replace(/J/g, '9');
        }
        
        var yInGrid = parseInt(ship.locations[0].location.slice(0, 1));
        var xInGrid = parseInt(ship.locations[0].location.slice(1, 3)) - 1;

        if (ship.position === "Horizontal") {
            grid.addWidget($('<div id="' + ship.type + '"><div class="grid-stack-item-content ' + ship.type + 'Horizontal"></div><div/>'),
                xInGrid, yInGrid, ship.locations.length, 1, false);
        } else if (ship.position === "Vertical") {
            grid.addWidget($('<div id="' + ship.type + '"><div class="grid-stack-item-content ' + ship.type + 'Vertical"></div><div/>'),
                xInGrid, yInGrid, 1, ship.locations.length, false);
        }
    })
}

function placeSalvos(salvos, playerId, ships) {
    salvos = JSON.parse(JSON.stringify(salvos));
    const shitPositions = [];
    ships.forEach(ship => ship.locations.forEach(location => { shitPositions.push(location.location) }))

    salvos.forEach(salvo => {
        if (salvo.player.id == playerId) {
            salvo.locations.forEach(location => {
                $('#' + location.location).addClass("shot");
                $('#' + location.location).text(salvo.turn);
            })
        }
        else {
            salvo.locations.forEach(location => {
                if (shitPositions.indexOf(location.location) != -1) {
                    location.location = location.location.replace(/A/g, '0');
                    location.location = location.location.replace(/B/g, '1');
                    location.location = location.location.replace(/C/g, '2');
                    location.location = location.location.replace(/D/g, '3');
                    location.location = location.location.replace(/E/g, '4');
                    location.location = location.location.replace(/F/g, '5');
                    location.location = location.location.replace(/G/g, '6');
                    location.location = location.location.replace(/H/g, '7');
                    location.location = location.location.replace(/I/g, '8');
                    location.location = location.location.replace(/J/g, '9');

                    console.log(parseInt(location.location.slice(0, 1)))
                    var yInGrid = (parseInt(location.location.slice(0, 1)) * 40) + 42;
                    var xInGrid = ((parseInt(location.location.slice(1, 3)) - 1) * 40) + 42;
                    $('.grid-ships').append('<div class="hitSelf" style="top:' + yInGrid + 'px; left:' + xInGrid + 'px;" ></div>');
                }
            })
        }
    })
}

function addEvents() {
    $("#Carrier, #PatroalBoat, #Submarine, #Destroyer, #BattleShip").click(function () {
        var h = parseInt($(this).attr("data-gs-height"));
        var w = parseInt($(this).attr("data-gs-width"));
        var posX = parseInt($(this).attr("data-gs-x"));
        var posY = parseInt($(this).attr("data-gs-y"));

        if (w > h) {
            if (grid.isAreaEmpty(posX, posY + 1, h, w - 1) && posY + w <= 10) {
                grid.update($(this), posX, posY, h, w);
                $(this).children('.grid-stack-item-content').removeClass($(this).attr('id') + "Horizontal");
                $(this).children('.grid-stack-item-content').addClass($(this).attr('id') + "Vertical");
            }
        } else {
            if (grid.isAreaEmpty(posX + 1, posY, h - 1 , w) && posX + h <= 10) {
                grid.update($(this), posX, posY, h, w);
                $(this).children('.grid-stack-item-content').addClass($(this).attr('id') + "Horizontal");
                $(this).children('.grid-stack-item-content').removeClass($(this).attr('id') + "Vertical");
            }
        }
    });
}