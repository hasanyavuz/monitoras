import Vue from 'vue'
import axios from 'axios'
import router from './router/index'
import store from './store'
import { sync } from 'vuex-router-sync'
import App from 'components/root/app-root'
import { FontAwesomeIcon } from './icons'
import PageHead from 'components/shared/page-head'
import Notifications from 'vue-notification'

import VueContentPlaceholders from 'vue-content-placeholders'

Vue.use(VueContentPlaceholders)

// Input Controls
import MTIText from 'components/input/text'

Vue.use(Notifications)

// Registration of global components
Vue.component('icon', FontAwesomeIcon)
Vue.component('page-head', PageHead)

// Registration for input controls
Vue.component('mti-text', MTIText)

Vue.prototype.$http = axios

sync(store, router)

const app = new Vue({
  store,
  router,
  ...App
})

export {
  app,
  router,
  store
}
