<template>
  <v-row align="center" justify="center">
    <v-col cols="12" sm="8" md="6" lg="4">
      <v-card raised>
        <v-card-title>Setting new password</v-card-title>
        <v-card-text v-if="!success">
          <error-info :error="error" />
          <v-form v-model="valid" autocomplete="off">
            <v-text-field
              v-model="password"
              autocomplete="new-password"
              :type="showPassword ? 'text' : 'password'"
              :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
              label="New password"
              :rules="[required('password'), minLength('password', 6)]"
              @click:append="showPassword = !showPassword"
            />
            <v-btn block color="primary" :disabled="!valid" @click="setPassword"
              >Set new password</v-btn
            >
          </v-form>
        </v-card-text>
        <v-card-text v-else>
          <v-alert type="success" dense>
            Your new password has been set. Please sign in.
          </v-alert>
          <v-btn color="primary" block to="/auth/signIn">Sign in</v-btn>
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
      error: null,
      password: '',
      showPassword: false,
      success: '',
      ...validations
    }
  },
  methods: {
    async setPassword() {
      try {
        const res = await this.$axios.post('/api/auth/resetPassword', {
          uid: this.$route.query.uid,
          code: this.$route.query.code,
          password: this.password
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
