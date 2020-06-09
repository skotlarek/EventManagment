<template>
  <v-form v-model="valid" autocomplete="off">
    <v-text-field
      v-model="userInfo.email"
      label="Adres email"
      :rules="[required('email'), emailFormat()]"
    />

    <v-btn
      block
      color="primary"
      :disabled="!valid"
      @click="submitForm(userInfo)"
      ><v-icon left>fa-user-ninja</v-icon>Kontynuuj</v-btn
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
      valid: false
    }
  }
}
</script>
<style lang="scss" scoped></style>
