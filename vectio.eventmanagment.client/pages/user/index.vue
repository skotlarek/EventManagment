<template>
  <div>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="10" md="8">
        <v-card raised outlined>
          <vuetify-logo />
        </v-card>
      </v-col>
    </v-row>
    <v-row align="center" justify="center">
      <v-col v-if="isAdmin" cols="12" sm="5" md="4" lg="3" xl="2">
        <v-card class="text-center" raised to="/admin/manageUsers">
          <v-card-text>
            <v-icon size="64">fa-user-cog</v-icon>
          </v-card-text>
          <v-card-actions>
            <v-btn block color="primary">Użytkownicy</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
      <v-col v-if="isAdmin" cols="12" sm="5" md="4" lg="3" xl="2">
        <v-card class="text-center" raised to="/admin/manageEvents">
          <v-card-text>
            <v-icon size="64">fa-file-medical</v-icon>
          </v-card-text>
          <v-card-actions>
            <v-btn block color="primary">Wydarzenia</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import VuetifyLogo from '~/components/VuetifyLogo.vue'
export default {
  components: {
    VuetifyLogo
  },
  middleware: 'auth',
  computed: {
    isAdmin() {
      return (
        this.$auth.loggedIn && this.$auth.user.roles.includes('administrator')
      )
    }
  }
}
</script>

<style lang="scss" scoped></style>
