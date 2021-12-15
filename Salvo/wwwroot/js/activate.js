const url = new URLSearchParams(window.location.search);
const urlParams = url.get('user');

var app = new Vue({
    el: "#app",
    data: {
        email: "",
        code: "",
        result:"",
        modal: {
            tittle: "",
            message: ""
        }
    },
    mounted(){
        let params = urlParams.split("/");
        if(params.length < 2){
            this.result = "Formato de url invalido"
            return;
        }
        this.email = params[0];
        this.code = params[1];
        axios.post("/api/players/activate/" + this.email + "/" + this.code)
            .then(resp =>{
                console.log("entre")
                if(resp.status == 200)
                    this.result = "Usuario activado"
                else
                    this.result = resp.data;
            })
            .catch(error =>{
                this.modal.tittle = "Error " + error.response.status;
                this.modal.message = error.response.data;
                console.log(error.response.data);
                $("#infoModal").modal('show');
            })
    },
    methods: {

    }
})