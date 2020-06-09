<template>
  <div>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="10" md="8">
        <v-card raised outlined>
          <vuetify-logo />
        </v-card>
      </v-col>
    </v-row>
    <v-card outlined raised>
      <v-card-title> Rejestracja - {{ dataevent.eventName }} </v-card-title>

      <v-form
        v-if="dataevent.remainingSeats !== 0"
        ref="formRegistration"
        v-model="valid"
      >
        <v-card-text>
          <div>
            <v-row>
              <v-col cols="4">
                <v-text-field
                  v-model="registration.firstName"
                  label="Imię"
                  outlined
                  dense
                  :rules="[required('firstname')]"
                />

                <v-text-field
                  v-model="registration.lastName"
                  label="Nazwisko"
                  outlined
                  dense
                />
                <v-text-field
                  v-model="registration.companyName"
                  label="Nazwa firmy"
                  outlined
                  dense
                />
                <v-text-field
                  v-model="registration.phone"
                  label="Telefon"
                  outlined
                  dense
                />
                <v-text-field
                  v-model="registration.email"
                  label="Email"
                  outlined
                  dense
                  :rules="[required('email'), emailFormat()]"
                />
              </v-col>
              <v-col cols="8">
                <div v-html="dataevent.content"></div>
              </v-col>
            </v-row>
          </div>
        </v-card-text>
        <v-card-actions>
          <v-btn
            color="primary"
            :disabled="!valid"
            @click.stop="sendRegistration"
            >Wyślij zgłoszenie</v-btn
          >
        </v-card-actions>
      </v-form>
      <v-card-text v-else>
        Z przykrością zawiadamiamy, że nie ma już wiecej wolnych miejsc
      </v-card-text>
    </v-card>
  </div>
</template>
<script>
import VuetifyLogo from '~/components/VuetifyLogo.vue'
import validations from '@/utils/validators'
export default {
  components: {
    VuetifyLogo
  },

  async asyncData({ $axios, params }) {
    const { data } = await $axios.get('/api/reg/' + params.id)
    return {
      valid: true,
      ...validations,
      dataevent: data,
      registration: {
        event: {},
        firstName: '',
        lastName: '',
        phone: '',
        companyName: '',
        email: '',
        eventId: data.id
      }
    }
  },
  data() {
    return {}
  },
  methods: {
    async sendRegistration() {
      try {
        const result = await this.$axios.post(
          '/api/sendregistration',
          this.registration
        )

        if (result) this.$router.push('/user/registration/success')
        else
          alert(
            ' Z przykrością zawiadamiamy, że nie ma już wiecej wolnych miejsc'
          )
      } catch {}
    }
  }
}
</script>
