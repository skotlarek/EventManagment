import colors from 'vuetify/es5/util/colors'
const development = process.env.NODE_ENV !== 'production'
export default {
  mode: 'spa',
  router: {
    // base: '' // for base path
  },
  /*
   ** Headers of the page
   */
  head: {
    titleTemplate: '%s - ' + process.env.npm_package_name,
    title: process.env.npm_package_name || '',
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      {
        hid: 'description',
        name: 'description',
        content: process.env.npm_package_description || ''
      }
    ],
    link: [{ rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }]
  },
  /*
   ** Customize the progress-bar color
   */
  loading: { color: 'blue', height: '6px' },
  /*
   ** Global CSS
   */
  css: [],
  /*
   ** Plugins to load before mounting the App
   */
  plugins: ['~/plugins/axios-auth-redirect', '~/plugins/axios-loading'],
  /*
   ** Nuxt.js dev-modules
   */
  buildModules: [
    // Doc: https://github.com/nuxt-community/eslint-module
    '@nuxtjs/eslint-module',
    '@nuxtjs/vuetify'
  ],
  /*
   ** Nuxt.js modules
   */
  modules: [
    // Doc: https://axios.nuxtjs.org/usage
    '@nuxtjs/axios',
    '@nuxtjs/auth'
  ],
  /*
   ** Axios module configuration
   ** See https://axios.nuxtjs.org/options
   */
  axios: {
    // baseURL: 'https://ariom.pl/v1'
    baseURL: development
      ? 'https://localhost:44373'
      : 'https://alumni.vectio.pl/v1'
    // proxy: true
  },
  // proxy: {
  //   '/api': {
  //     target: 'http://localhost:51722',
  //     secure: false
  //   }
  // },
  /*
   ** vuetify module configuration
   ** https://github.com/nuxt-community/vuetify-module
   */
  vuetify: {
    customVariables: ['~/assets/variables.scss'],
    theme: {
      dark: false,
      themes: {
        dark: {
          primary: colors.blue.darken2,
          accent: colors.grey.darken3,
          secondary: colors.amber.darken3,
          info: colors.teal.lighten1,
          warning: colors.amber.base,
          error: colors.deepOrange.accent4,
          success: colors.green.accent3
        }
      }
    },
    defaultAssets: {
      font: {
        family: 'Roboto'
      },
      icons: 'fa'
    }
  },
  auth: {
    redirect: {
      login: '/auth/signIn',
      logout: '/',
      callback: false,
      home: false
    },
    strategies: {
      local: {
        endpoints: {
          login: {
            url: '/api/auth/login',
            method: 'post',
            propertyName: 'token',
            tokenRequired: true
          },
          logout: false,
          user: { url: '/api/auth/user', method: 'get', propertyName: 'user' }
        },
        // tokenRequired: true,
        tokenType: 'bearer',
        autoFetchUser: true
      }
    }
  },
  /*
   ** Build configuration
   */
  build: {
    /*
     ** You can extend webpack config here
     */
    extend(config, ctx) {}
  }
}
