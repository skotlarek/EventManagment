<template>
  <div>
    <v-card outlined raised>
      <v-card-title>
        <v-btn class="mr-2" small to="/admin/manageEvents" color="primary"
          ><v-icon left>fa-arrow-left</v-icon>Powrót do listy</v-btn
        >
        {{ event.id ? 'Edycja' : 'Dodawanie' }} wydarzenia
      </v-card-title>

      <v-form ref="formEvent">
        <v-card-text>
          <div>
            <v-row>
              <v-col cols="5">
                <v-text-field
                  v-model="event.eventName"
                  label="Nazwa"
                  outlined
                  dense
                />
                <v-row dense>
                  <v-col cols="4">
                    <div class="ml-auto">
                      <v-menu
                        v-model="date"
                        class="mr-2"
                        :close-on-content-click="false"
                        :nudge-right="40"
                        transition="scale-transition"
                        offset-y
                        min-width="290px"
                      >
                        <template v-slot:activator="{ on }">
                          <v-text-field
                            v-model="event.eventDate"
                            label="Data wydarzenia"
                            prepend-icon="fa-calendar-alt"
                            clearable
                            v-on="on"
                          ></v-text-field>
                        </template>
                        <v-date-picker
                          v-model="event.eventDate"
                          locale="pl"
                          @input="date = false"
                        ></v-date-picker>
                      </v-menu>
                    </div>
                  </v-col>
                  <v-col cols="4">
                    <div class="ml-auto">
                      <v-menu
                        ref="menu"
                        v-model="menu2"
                        class="mr-2"
                        :close-on-content-click="false"
                        :nudge-right="40"
                        :return-value.sync="event.eventTime"
                        transition="scale-transition"
                        offset-y
                        min-width="290px"
                      >
                        <template v-slot:activator="{ on }">
                          <v-text-field
                            v-model="event.eventTime"
                            label="Godzina wydarzenia"
                            prepend-icon="fa-clock"
                            v-on="on"
                          ></v-text-field>
                        </template>
                        <v-time-picker
                          v-if="menu2"
                          v-model="event.eventTime"
                          format="24hr"
                          full-width
                          @click:minute="$refs.menu.save(event.eventTime)"
                        ></v-time-picker>
                      </v-menu>
                    </div>
                  </v-col>
                  <v-col cols="4">
                    <v-text-field
                      v-model.number="event.durationTime"
                      label="Szacowany czas(w min.)"
                      outlined
                      dense
                  /></v-col>
                </v-row>

                <v-row dense>
                  <v-col cols="8"
                    ><v-switch
                      v-model="event.limitedPlaces"
                      dense
                      class="ma-0"
                      label="Liczba miejsc ograniczona"
                    ></v-switch
                  ></v-col>
                  <v-col cols="4"
                    ><template v-if="event.limitedPlaces">
                      <v-text-field
                        v-model.number="event.numberSeats"
                        label="Liczba miejsc"
                        outlined
                        dense
                      /> </template
                  ></v-col>
                </v-row>
              </v-col>
              <v-col cols="7">
                <vue-editor v-model="event.content"></vue-editor
              ></v-col>
            </v-row>
          </div>
        </v-card-text>
      </v-form>
      <v-card-actions>
        <v-btn color="primary" @click.stop="save">Zapisz zmiany</v-btn>
      </v-card-actions>
    </v-card>
  </div>
</template>
<script>
import { VueEditor } from 'vue2-editor'

export default {
  components: { VueEditor },
  async asyncData({ $axios, params }) {
    if (params.id === 'new')
      return {
        event: {
          eventName: '',
          content: '',
          eventDate: null,
          durationTime: null,
          numberSeats: null
        }
      }
    const { data } = await $axios.get('/api/events/' + params.id)
    return {
      event: data
    }
  },

  data: () => ({
    date: false,
    menu2: false
  }),
  methods: {
    async save() {
      const date = this.event.eventDate + 'T' + this.event.eventTime
      this.event.eventDate = date
      if (!this.event.id) {
        await this.$axios.post('/api/events/', this.event)
      } else {
        await this.$axios.put('/api/events/' + this.event.id, this.event)
      }
      this.$router.push('/admin/manageEvents')
    }
  }
}
</script>
