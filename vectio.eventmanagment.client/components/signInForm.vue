<template>
  <v-form v-model="valid" autocomplete="off">
    <v-text-field
      v-model="userInfo.email"
      label="Adres email"
      :rules="[required('email'), emailFormat()]"
    />

    <v-text-field
      v-model="userInfo.password"
      autocomplete="off"
      :type="showPassword ? 'text' : 'password'"
      :append-icon="showPassword ? 'fa-eye' : 'fa-eye-slash'"
      label="Hasło"
      :rules="[required('password'), minLength('password', 6)]"
      @click:append="showPassword = !showPassword"
    />

    <v-btn
      block
      color="primary"
      :disabled="!valid"
      @click="submitForm(userInfo)"
      ><v-icon left>fa-user-check</v-icon>Zaloguj się</v-btn
    >
    <div class="text-right">
      <v-btn color="primary" text to="/auth/forgotPassword"
        >Zapomniałem hasła...</v-btn
      >
    </div>
  </v-form>
</template>

<script>
import validators from '@/utils/validators.js'
export default {
  props: { submitForm: { type: Function, required: true } },
  data() {
    return {
      ...validators,
      valid: false,
      showPassword: false,
      userInfo: {
        email: '',
        password: '',
        fullname: ''
      }
    }
  }
}
</script>
<style lang="scss" scoped></style>
