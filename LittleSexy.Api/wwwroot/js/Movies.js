


var vueMovieList=new Vue({
  el:'#app',
  data:{
    movieList:[]
  },        
  mounted() {
    this.init()
  },
  methods:{
    init:function () {
      let _self = this;

      axios.get('api/Movie/List')
          .then(function (response) {

              var apiData = response.data.content;
              _self.movieList = apiData;
              //debugger;
              console.log(_self.movieList);
          })
          .catch(function (error) {

              console.log(error);
          });
    }
  }
});
// var Ctor = Vue.extend(Main)
vueMovieList.$mount('#app');