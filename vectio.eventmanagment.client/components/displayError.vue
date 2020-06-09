<template>
  <v-snackbar v-model="show" top color="error" :timeout="timeout"
    >Wystąpił problem z połączeniem sieciowym. Spróbuj ponownie.
    <v-btn text @click="show = false">
      Close
    </v-btn>
  </v-snackbar>
</template>

<script>
import { mapState, mapActions } from 'vuex'
export default {
  data() {
    return {
      visible: true,
      timeout: 3000
    }
  },
  computed: {
    ...mapState({
      error: (state) => state.networkError
    }),
    show: {
      get() {
        return this.error !== null
      },
      set(value) {
        this.setError(null)
      }
    }
  },
  methods: mapActions(['setError'])
}
</script>

<style lang="scss" scoped></style>
