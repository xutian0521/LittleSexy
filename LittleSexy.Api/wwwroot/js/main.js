
    function sleep(numberMillis) {
        var now = new Date();
        var exitTime = now.getTime() + numberMillis;
        while (true) {
            now = new Date();
            if (now.getTime() > exitTime)
                return;
        }
    }
    var test = {
        data: () => ({
            show: false

        })  
    }
    var Ctor2 = Vue.extend(test)
    new Ctor2().$mount('#test')

    //head handler
    var vueHead = new Vue({
        el: '#_head',
        data: {
            title: "little sexy title",
            description: "this is a sexy site.",
            author: "xutian"

        },
        mounted() {
            this.init()
        },
        methods: {
            init() {
                console.log('hello,world')
                let _self = this;

                axios.get('api/Page/Index')
                    .then(function (response) {
                        //debugger;

                        console.log(response);

                        var apiData = response.data.content;

                        _self.title = apiData.title;
                        _self.description = apiData.description;
                        _self.author = apiData.author;
                    })
                    .catch(function (error) {

                        console.log(error);
                    });
            }
        }
    });
    vueHead.$mount('#_head');
    //navigation handler
    var Main = {
        data() {
            return {
                activeIndex: '1',
            };
        },
        methods: {
            handleSelect(key, keyPath) {
                console.log(key, keyPath);
            }
        }
    }
    var Ctor = Vue.extend(Main);
    new Ctor().$mount('#navMenu');

    //banner handler
    var vueApp = new Vue({
        el: '#banner',
        data: {
            show: false,
            bannerList:[]

        },
        mounted() {
            this.init()
        },
        methods: {
            init() {
                let _self = this;

                axios.get('api/Page/Images?pageId=1')
                    .then(function (response) {
                        //debugger;

                        console.log(response);

                        var apiDataArr = response.data.content;

                        _self.bannerList = apiDataArr;
                    })
                    .catch(function (error) {

                        console.log(error);
                    });
            }
        }
    });
    vueApp.$mount('#banner');

