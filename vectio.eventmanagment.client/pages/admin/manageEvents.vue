<template>
  <div>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title>
            <v-btn large icon color="primary" to="/"
              ><v-icon>fa-home</v-icon></v-btn
            >
            Lista wydarzeń
          </v-card-title>
          <v-card-actions>
            <v-btn
              color="primary"
              outlined
              small
              class="my-1"
              to="/admin/event/new"
              ><v-icon left>fa-plus</v-icon>Dodaj</v-btn
            >
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
            :items="events"
            :loading="loading"
            :search="search"
          >
            <template v-slot:item.id="{ item }">
              <v-btn x-small color="primary" :to="'/admin/event/' + item.id"
                >Szczegóły</v-btn
              >
            </template>
            <template v-slot:item.link="{ item }">
              <v-btn
                v-clipboard:copy="item.link"
                v-clipboard:success="onCopy"
                v-clipboard:error="onError"
                x-small
                color="primary"
              >
                Link
              </v-btn>
            </template>
            <template v-slot:item.users="{ item }">
              <v-btn
                x-small
                color="success"
                :to="'/admin/eventUsers/' + item.id"
                >Uczestnicy</v-btn
              >
            </template>
          </v-data-table>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import Vue from 'vue'
import VueClipboard from 'vue-clipboard2'

VueClipboard.config.autoSetContainer = true // add this line
Vue.use(VueClipboard)

export default {
  components: {},
  middleware: ['is-admin'],
  data() {
    return {
      error: null,
      events: [],
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
        { text: '', value: 'id', width: '15em' },

        {
          text: 'Data wydarzenia',
          value: 'eventDate',
          align: 'right',
          width: '10em'
        },
        {
          text: 'Godzina wydarzenia',
          value: 'eventTime',
          align: 'right',
          width: '15em'
        },
        { text: 'Nazwa wydarzenia', value: 'eventName' },
        { text: 'Liczba miejsc', value: 'numberSeats' },
        { text: 'Pozostało wolnych miejsc', value: 'remainingSeats' },
        {
          text: 'Data utw. wpisu',
          value: 'createdAt',
          align: 'right',
          width: '15em'
        },
        { text: 'Link rejestracji', value: 'link' },
        { text: 'Uczestnicy', value: 'users' }
      ]
    }
  },
  computed: {
    newEvent() {
      return this.currentEvent.id === null
    }
  },
  async mounted() {
    await this.loadEvents()
  },
  methods: {
    async loadEvents() {
      const { data } = await this.$axios.get('/api/events/events')
      this.events = data
    },
    async showDetails(id) {
      this.error = null
      this.currentEvent = null
      this.showEvent = true
      const { data } = await this.$axios.get('/api/events/event/' + id)
      this.currentEvent = data.event
    },
    onCopy(e) {
      alert('Skopiowano: ' + e.text)
    },
    onError(e) {
      alert('Nie udało sie ')
    }
  }
}
</script>

<style lang="scss" scoped></style>
