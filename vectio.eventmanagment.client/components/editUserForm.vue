<template>
  <v-form v-model="valid" autocomplete="off">
    <v-text-field
      v-model="user.firstname"
      label="Imię"
      :rules="[required('firstname')]"
    />
    <v-text-field
      v-model="user.lastname"
      label="Nazwisko"
      :rules="[required('lastname')]"
    />
    <v-text-field
      v-model="user.email"
      label="Adres e-mail"
      :rules="[required('email'), emailFormat()]"
    />
    <!-- <template v-if="newUser">
      <v-text-field
        v-model="user.password"
        autocomplete="new-password"
        :type="showPassword ? 'text' : 'password'"
        :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
        label="Hasło"
        :rules="[minLengthOrEmpty('password', 6)]"
        @click:append="showPassword = !showPassword"
      />
    </template> -->
    <v-radio-group v-model="role" dense row :rules="[required('role')]">
      <v-radio label="administrator" value="administrator"></v-radio>
      <!-- <v-radio label="użytkownik" value="user"></v-radio> -->
    </v-radio-group>
    <v-spacer />
    <v-row>
      <v-col
        ><v-checkbox
          v-model="user.emailConfirmed"
          dense
          label="E-mail potwierdzony"
        ></v-checkbox>
        <v-btn
          v-if="user.id && !user.emailConfirmed && valid"
          x-small
          outlined
          color="primary"
          @click="$emit('send-email', user)"
          >Wyślij ponownie email z linkiem aktywacyjnym</v-btn
        >
      </v-col>
    </v-row>

    <v-btn
      block
      color="primary"
      :disabled="!valid"
      @click="$emit('saving-user', user)"
    >
      {{ newUser ? 'Utwórz konto' : 'Zapisz zmiany' }}</v-btn
    >
  </v-form>
</template>

<script>
import validations from '@/utils/validators'
export default {
  props: { user: { type: Object, required: true } },
  data() {
    return {
      valid: true,
      showPassword: false,
      ...validations
    }
  },
  computed: {
    newUser() {
      return this.user.id === null
    },
    role: {
      get() {
        return this.user.roles[0]
      },
      set(v) {
        this.user.roles[0] = v
      }
    }
  },
  methods: {
    sendActivationEmail() {}
  }
}
</script>

<style lang="scss" scoped></style>
