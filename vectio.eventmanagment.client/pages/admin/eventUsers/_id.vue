<template>
  <div>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title>
            <v-btn large icon color="primary" to="/"
              ><v-icon>fa-home</v-icon></v-btn
            >
            Lista uczestników
          </v-card-title>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-text-field
              v-model="search"
              append-icon="mdi-magnify"
              label="szukaj..."
              single-line
              hide-details
              clearable
            ></v-text-field>
          </v-card-actions>
          <v-data-table
            dense
            loading-text="proszę czekać...ładowanie danych..."
            :headers="columns"
            :items="users"
            :loading="loading"
            :search="search"
          >
          </v-data-table>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script>
export default {
  components: {},
  middleware: ['is-admin'],
  async asyncData({ $axios, params }) {
    const { data } = await $axios.get('/api/eventUsers/' + params.id)
    return {
      users: data
    }
  },
  data() {
    return {
      error: null,
      users: [],
      search: '',
      lastSearch: '',
      lastRef: null,
      lastPage: 1,
      loading: false,
      totalRows: 0,

      snack: {
        show: false,
        msg: '',
        color: 'success',
        timeout: 2000
      },
      showEvent: false,
      currentEvent: null,
      columns: [
        { text: 'Imię', value: 'firstname' },
        { text: 'Nazwisko', value: 'lastname' },
        { text: 'Nazwa firmy', value: 'companyName' },
        { text: 'Adres email', value: 'email' },
        { text: 'Telefon', value: 'phone' }
      ]
    }
  }
}
</script>

<style lang="scss" scoped></style>
