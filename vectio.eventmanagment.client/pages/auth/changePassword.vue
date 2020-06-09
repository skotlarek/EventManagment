<template>
  <div>
    <v-btn color="primary" outlined small class="my-1" to="/">
      <v-icon>fa-home</v-icon></v-btn
    >
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="6" lg="4">
        <v-card raised>
          <v-card-title>Zmiana hasła</v-card-title>
          <v-card-text v-if="!success">
            <error-info :error="error" />
            <v-form v-model="valid" autocomplete="off">
              <v-text-field
                v-model="oldPassword"
                autocomplete="new-password"
                :type="showPassword1 ? 'text' : 'password'"
                :append-icon="showPassword1 ? 'mdi-eye' : 'mdi-eye-off'"
                label="Bieżące hasło"
                :rules="[required('password'), minLength('password', 6)]"
                @click:append="showPassword1 = !showPassword1"
              />
              <v-text-field
                v-model="newPassword"
                autocomplete="new-password"
                :type="showPassword2 ? 'text' : 'password'"
                :append-icon="showPassword2 ? 'mdi-eye' : 'mdi-eye-off'"
                label="Nowe hasło"
                :rules="[required('password'), minLength('password', 6)]"
                @click:append="showPassword2 = !showPassword2"
              />
              <v-btn
                block
                color="primary"
                :disabled="!valid"
                @click="setPassword"
                >Zapisz nowe hasło</v-btn
              >
            </v-form>
          </v-card-text>
          <v-card-text v-else>
            <v-alert type="success" dense>
              Nowe hasło zapisano.
            </v-alert>
            <v-btn color="primary" block to="/">Ok</v-btn>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import errorInfo from '@/components/errorInfo.vue'
import validations from '@/utils/validators'
export default {
  components: { errorInfo },
  data() {
    return {
      valid: false,
      error: null,
      oldPassword: '',
      newPassword: '',
      showPassword1: false,
      showPassword2: false,
      success: '',
      ...validations
    }
  },
  methods: {
    async setPassword() {
      try {
        const res = await this.$axios.post('/api/auth/changePassword', {
          uid: this.$auth.user.id,
          oldPassword: this.oldPassword,
          newPassword: this.newPassword
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
