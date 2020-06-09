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
      <v-card color="green">
        <v-card-text>
          <div>
            Gratulacje. Miło nam potwierdzić Twoją rejestrację na wydarzenie.
          </div>
        </v-card-text>
      </v-card>
    </v-row>
  </div>
</template>

<script>
import VuetifyLogo from '~/components/VuetifyLogo.vue'
export default {
  components: {
    VuetifyLogo
  },

  computed: {}
}
</script>

<style lang="scss" scoped></style>
