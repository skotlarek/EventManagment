<template>
  <div>
    <v-row>
      <v-col cols="12">
        <users-list
          :users="users"
          @show-details="showDetails"
          @add-new="createNewUser"
        ></users-list>
      </v-col>
    </v-row>
    <v-dialog v-model="showUser" max-width="50em">
      <v-card v-if="currentUser">
        <v-card-title
          >{{
            newUser
              ? 'Tworzenie nowego konta użytkownika'
              : 'Edycja konta użytkownika'
          }}
          <v-spacer></v-spacer>
          <v-btn color="primary" text @click="showUser = false">
            <v-icon left>fa-times</v-icon>
            Zamknij
          </v-btn>
        </v-card-title>
        <v-card-text>
          <error-info :error="error" />
          <edit-user-form
            :user="currentUser"
            @saving-user="savingUser"
            @send-email="sendEmail"
          ></edit-user-form>
        </v-card-text>
      </v-card>
      <v-card v-else><v-card-title> ładowanie...</v-card-title></v-card>
    </v-dialog>

    <v-snackbar
      v-model="snack.show"
      :timeout="snack.timeout"
      :color="snack.color"
      top
    >
      {{ snack.msg }}
      <v-btn color="text-success" text @click="snack.show = false">
        Close
      </v-btn>
    </v-snackbar>
  </div>
</template>

<script>
import errorInfo from '@/components/errorInfo.vue'
import usersList from '@/components/usersList.vue'
import editUserForm from '@/components/editUserForm.vue'
export default {
  components: { usersList, editUserForm, errorInfo },
  middleware: ['is-admin'],

  data() {
    return {
      error: null,

      users: [],
      snack: {
        show: false,
        msg: '',
        color: 'success',
        timeout: 2000
      },
      showUser: false,
      currentUser: null
    }
  },
  computed: {
    newUser() {
      return this.currentUser.id === null
    }
  },
  async mounted() {
    await this.loadUsers()
  },
  methods: {
    async loadUsers() {
      const { data } = await this.$axios.get('/api/auth/users')
      this.users = data.users
    },
    async showDetails(id) {
      this.error = null
      this.currentUser = null
      this.showUser = true
      const { data } = await this.$axios.get('/api/auth/user/' + id)
      this.currentUser = data.user
    },
    createNewUser() {
      this.error = null
      this.currentUser = {
        id: null,
        email: '',
        fullname: '',
        roles: ['administrator']
      }
      this.showUser = true
    },
    async sendEmail(user) {
      this.error = null
      if (user.id) {
        // create new user
        try {
          await this.$axios.post('/api/auth/sendActivationEmail', {
            callbackUrl: 'https://alumni.vectio.pl' + '/auth/confirmEmail',
            uid: user.id
          })
          this.snack = {
            show: true,
            msg: 'Wysłano mail aktywacyjny',
            color: 'success',
            timeout: 2000
          }
        } catch (error) {
          this.error = error
        }
      }
    },
    async savingUser(user) {
      this.error = null

      if (!user.id) {
        // create new user
        try {
          await this.$axios.post('/api/auth/createUser', {
            confirmationUrl: 'https://alumni.vectio.pl' + '/auth/confirmEmail',
            ...user
          })

          await this.loadUsers()
          this.showUser = false
          this.snack = {
            show: true,
            msg: 'Utworzono konto użytkownika',
            color: 'success',
            timeout: 2000
          }
        } catch (error) {
          this.error = error
        }
      } else {
        // edit existing
        try {
          await this.$axios.put('/api/auth/user', user)
          await this.loadUsers()
          this.showUser = false
          this.snack = {
            show: true,
            msg: 'Zapisano zmiany',
            color: 'success',
            timeout: 2000
          }
        } catch (error) {
          this.error = error
        }
      }
    }
  }
}
</script>

<style lang="scss" scoped></style>
