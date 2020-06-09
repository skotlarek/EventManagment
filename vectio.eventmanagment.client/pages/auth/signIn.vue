<template>
  <v-row align="center" justify="center">
    <v-col cols="12" sm="8" md="6" lg="4">
      <v-card raised>
        <v-card-title>Zaloguj się</v-card-title>
        <v-card-text>
          <error-info :error="error" />
          <sign-in-form :submit-form="signIn" :user-info="userInfo" />
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>
<script>
import errorInfo from '@/components/errorInfo.vue'
import signInForm from '@/components/signInForm.vue'
export default {
  components: { signInForm, errorInfo },
  data() {
    return {
      error: null,
      userInfo: {
        email: '',
        password: '',
        fullname: null,
        role: null
      }
    }
  },
  methods: {
    async signIn(userInfo) {
      try {
        this.error = {}
        await this.$auth.loginWith('local', {
          data: {
            username: userInfo.email,
            password: userInfo.password
          }
        })
        this.$router.push('/user/home')
      } catch (e) {
        if (e.response && e.response.status === 401)
          this.error = { message: 'Nie poprawne hasło lub login' }
        else this.error = e
      }
    }
  }
}
</script>

<style lang="scss" scoped></style>
