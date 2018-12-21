

var Main = {
    data() {
      return {
        tableData: [{
          date: '2016-05-02',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1518 弄'
        }, {
          date: '2016-05-04',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1517 弄'
        }, {
          date: '2016-05-01',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1519 弄'
        }, {
          date: '2016-05-03',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1516 弄'
        }]
      }
    }
  }
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