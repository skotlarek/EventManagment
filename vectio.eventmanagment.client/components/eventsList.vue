<template>
  <div>
    <v-card>
      <v-card-title>
        <v-btn large icon color="primary" to="/"
          ><v-icon>fa-home</v-icon></v-btn
        >
        Lista Ewentów
      </v-card-title>
      <v-card-actions>
        <v-btn
          color="primary"
          outlined
          small
          class="my-1"
          @click="$emit('add-new')"
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
        loading-text="pobieranie danych..."
        :headers="columns"
        :items="events"
        :options.sync="options"
        :loading="loading"
        :search="search"
      >
        <template v-slot:item.id="{ item }">
          <v-btn x-small color="primary" @click="$emit('show-details', item.id)"
            >Szczegóły</v-btn
          >
        </template>
      </v-data-table>
    </v-card>
  </div>
</template>

<script>
export default {
  props: { events: { type: Array, required: true } },
  data() {
    return {
      loading: false,
      search: '',

      totalRows: 0
    }
  }
}
</script>

<style lang="scss" scoped></style>
