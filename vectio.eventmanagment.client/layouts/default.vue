<template>
  <v-app>
    <v-navigation-drawer
      v-model="drawer"
      :mini-variant="miniVariant"
      :clipped="clipped"
      fixed
      app
    >
      <v-list>
        <v-list-item>
          <v-list-item-action>
            <v-btn icon @click.stop="miniVariant = !miniVariant">
              <v-icon
                >fa-{{ `chevron-${miniVariant ? 'right' : 'left'}` }}</v-icon
              >
            </v-btn>
          </v-list-item-action>
        </v-list-item>
        <v-list-item
          v-for="(item, i) in items"
          :key="i"
          :to="item.to"
          router
          exact
        >
          <v-list-item-action>
            <v-icon>{{ item.icon }}</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title v-text="item.title" />
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
    <v-app-bar :clipped-left="clipped" fixed app>
      <v-img
        max-width="50"
        max-height="95%"
        contain
        :src="require('~/assets/img/vectio-logo.svg')"
      ></v-img>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer" />

      <v-toolbar-title v-text="title" />
      <v-spacer></v-spacer>

      <template v-if="!$store.state.auth.loggedIn">
        <v-btn color="primary" rounded outlined to="/auth/signIn"
          ><v-icon left>fa-sign-in-alt</v-icon>Zaloguj się</v-btn
        >
      </template>
      <template v-else>
        <v-btn
          fab
          small
          color="success darken-2"
          to="/auth/changePassword"
          :title="$auth.user.email"
        >
          <v-icon small>fa-user-cog</v-icon>
        </v-btn>
        <v-btn
          rounded
          outlined
          color="primary"
          class="mx-2"
          @click="$auth.logout()"
        >
          Wyloguj
          <v-icon right>fa-sign-out-alt</v-icon>
        </v-btn>
      </template>
    </v-app-bar>
    <v-content>
      <v-container>
        <v-row dense
          ><v-col cols="12" xl="10" offset-xl="1"> <nuxt /> </v-col
        ></v-row>
      </v-container>
    </v-content>
    <v-footer :fixed="fixed" app>
      <v-spacer></v-spacer>
      <span> &copy;{{ new Date().getFullYear() }} Vectio Sp. z o. o.</span>
    </v-footer>
  </v-app>
</template>

<script>
export default {
  components: {},
  data() {
    return {
      cartAmount: 0,
      clipped: true,
      drawer: false,
      fixed: false,

      miniVariant: true,
      title: 'Vectio'
    }
  },
  computed: {
    items() {
      const items = [
        {
          icon: 'fa-home',
          title: 'Start',
          to: '/user/home'
        }
      ]

      if (
        this.$auth.loggedIn &&
        this.$auth.user.roles.includes('administrator')
      ) {
        items.push({
          icon: 'fa-users',
          title: 'Użytkownicy',
          to: '/admin/manageUsers'
        })
        items.push({
          icon: 'fa-file-medical',
          title: 'Wydarzenia',
          to: '/admin/manageEvents'
        })
      }
      return items
    }
  }
}
</script>
