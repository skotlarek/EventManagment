<template>
  <div>
    <template v-if="!success && !error">
      <v-row align="center" justify="center">
        <v-col cols="12" sm="6" lg="4">
          <v-card raised>
            <v-card-title
              >Weryfikacja adresu e-mail
              <v-spacer></v-spacer>
              <span class="subtitle-1">(krok 1/2)</span>
            </v-card-title>
            <v-card-text
              >Kliknij "Potwierdź" w celu potwierdzenia utworzenia swojego konta
              w <strong>alumni.vectio.pl</strong>
            </v-card-text>
            <v-card-actions>
              <v-spacer />
              <v-btn block color="primary" @click="confirm()">Potwierdź</v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </template>
    <template v-if="success">
      <v-row align="center" justify="center">
        <v-col cols="12" sm="6" lg="4">
          <v-card class="text-center" raised>
            <v-card-title>
              <v-spacer></v-spacer>
              <span class="subtitle-1">(krok 2/2)</span></v-card-title
            >
            <v-card-text>
              <v-icon size="64">fa-check-circle</v-icon>
              <v-alert type="success">
                Twoje konto zostało utworzone i aktywowane</v-alert
              >
              <p class="subtitle-2">
                Trzeba jeszcze ustawić hasło:
              </p>
              <v-form v-model="valid" autocomplete="off">
                <error-info :error="activationError" />
                <v-text-field
                  v-model="password"
                  autocomplete="new-password"
                  :type="showPassword ? 'text' : 'password'"
                  :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                  label="podaj hasło..."
                  :rules="[required('password'), minLength('password', 6)]"
                  @click:append="showPassword = !showPassword"
                />
                <v-divider></v-divider>
                <v-btn
                  block
                  color="primary"
                  :disabled="!valid"
                  @click="setPassword()"
                  >Ustaw hasło i zaloguj się</v-btn
                >
              </v-form>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </template>
    <template v-if="error">
      <v-row align="center" justify="center">
        <v-col cols="12" sm="6" lg="4">
          <v-card class="text-center" raised>
            <v-card-text>
              <v-icon size="64">fa-check-circle</v-icon>
              <error-info :error="error" />
            </v-card-text>
            <v-card-actions>
              <v-btn color="primary" text to="/forgotPassword"
                >Zapomniałem hasła</v-btn
              >
              <v-spacer></v-spacer>
              <v-btn color="primary" to="/signIn">Zaloguj</v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </template>
  </div>
</template>

<script>
import errorInfo from '@/components/errorInfo.vue'
import validations from '@/utils/validators'
export default {
  middleware: ['auth'],
  options: {
    auth: false
  },
  components: { errorInfo },
  data() {
    return {
      valid: true,
      showPassword: false,
      error: '',
      activationError: '',
      success: '',
      password: '',
      ...validations
    }
  },
  methods: {
    async confirm() {
      try {
        const res = await this.$axios.post('/api/auth/confirmEmail', {
          uid: this.$route.query.uid,
          code: this.$route.query.code
        })

        this.success = res.data.message
      } catch (error) {
        this.error = error
      }
    },
    async setPassword() {
      try {
        this.activationError = ''
        const res = await this.$axios.post('/api/auth/setPassword', {
          uid: this.$route.query.uid,
          password: this.password
        })

        this.success = res.data.message
        this.$auth.loginWith('local', {
          data: {
            username: res.data.email,
            password: this.password
          }
        })
        this.$router.push('/')
        // .catch((e) => {this.activationError = e + ''})
      } catch (error) {
        this.activationError = error
      }
    }
  }
}
</script>

<style lang="scss" scoped></style>
