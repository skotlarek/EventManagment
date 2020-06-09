<template>
  <v-row align="center" justify="center">
    <v-col cols="12" sm="8" md="6" lg="4">
      <v-card raised>
        <v-card-title>Resetowanie hasła </v-card-title>
        <v-card-text>
          <p class="subtitle-1 my-0">
            Proszę wpisać swój adres email.
          </p>
          <p class="subtitle-1 my-0">
            Na podany adres prześlemy link umożliwiający zmianę hasła.
          </p>
        </v-card-text>
        <v-card-text v-if="!success">
          <error-info :error="error" />
          <v-form v-model="valid" autocomplete="off">
            <v-text-field
              v-model="email"
              label="Adres e-mail"
              :rules="[required('email'), emailFormat()]"
            />
            <v-btn
              block
              color="primary"
              :disabled="!valid"
              @click="resetPassword"
              >Resetuj hasło</v-btn
            >
          </v-form>
        </v-card-text>
        <v-card-text v-else>
          <v-alert type="success" dense>
            {{ success }}
          </v-alert>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
import errorInfo from '@/components/errorInfo.vue'
import validations from '@/utils/validators'
export default {
  components: { errorInfo },
  data() {
    return {
      valid: false,
      error: '',
      success: '',
      email: '',
      ...validations
    }
  },
  methods: {
    async resetPassword() {
      try {
        const res = await this.$axios.post('/api/auth/forgotPassword', {
          email: this.email,
          callbackUrl: 'https://alumni.vectio.pl' + '/auth/resetPassword'
        })

        this.success = res.data.message
      } catch (error) {
        this.error = error
      }
    }
  }
}
</script>

<style lang="scss" scoped></style>
