<template>
  <v-form v-model="valid" autocomplete="off">
    <v-row dense>
      <v-col>
        <v-text-field
          v-model="userInfo.firstname"
          label="Imię"
          :rules="[required('firstname')]"
      /></v-col>
      <v-col
        ><v-text-field
          v-model="userInfo.lastname"
          label="Nazwisko"
          :rules="[required('lastname')]"
      /></v-col>
    </v-row>

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
      ><v-icon left>fa-user-plus</v-icon>Zarejestruj się</v-btn
    >
  </v-form>
</template>

<script>
import validators from '@/utils/validators.js'
export default {
  props: {
    submitForm: { type: Function, required: true },
    userInfo: { type: Object, required: true }
  },
  data() {
    return {
      ...validators,
      valid: false,
      showPassword: false
    }
  }
}
</script>

<style lang="scss" scoped></style>
