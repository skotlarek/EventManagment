<template>
  <v-alert v-if="show" type="error" dense class="text-left">
    {{ message }}
    <ul v-if="issues">
      <li v-for="e in issues" :key="e">{{ e }}</li>
    </ul>
  </v-alert>
</template>

<script>
export default {
  props: { error: { type: Object, required: false, default: null } },
  computed: {
    show() {
      return (
        this.error &&
        ((this.error.response &&
          this.error.response.status === 409 &&
          this.error.response.data.result) ||
          this.error.message)
      )
    },
    message() {
      return this.error &&
        this.error.response &&
        this.error.response.status === 409
        ? this.error.response.data.result
        : this.error && this.error.message
        ? this.error.message
        : 'An error occurred'
    },
    issues() {
      return this.error &&
        this.error.response &&
        this.error.response.status === 409
        ? this.error.response.data.details.issues
        : this.error && this.error.issues
        ? this.error.issues
        : []
    }
  }
}
</script>

<style lang="scss" scoped></style>
