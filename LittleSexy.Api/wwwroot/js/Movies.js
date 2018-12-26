


var vueMovieList=new Vue({
  el:'#app',
  data:{
    movieList:[],
    total:'0',
    currentPage: 4
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
    },      
    handleSizeChange(val) {
      console.log(`每页 ${val} 条`);
    },
    handleCurrentChange(val) {
      console.log(`当前页: ${val}`);
    }
  }
  
});
// var Ctor = Vue.extend(Main)
vueMovieList.$mount('#app');