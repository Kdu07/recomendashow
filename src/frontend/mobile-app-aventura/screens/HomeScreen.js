import React from 'react';
import { View, StyleSheet, ScrollView } from 'react-native';

import HomeHeroBanner from '../components/HomeHeroBanner';
import CategoryChip from '../components/CategoryChip';
import SectionHeader from '../components/SectionHeader';
import PosterCard from '../components/PosterCard';
import TheaterCard from '../components/TheaterCard';
import EmptyState from '../components/EmptyState';
import BottomTabBar from '../components/BottomTabBar';

import useHomeViewModel from '../viewmodels/useHomeViewModel';

import { colors } from '../constants/colors';
import { spacing } from '../constants/spacing';

function PosterSection({ title, subtitle, data = [], onEventPress }) {
  return (
    <View style={styles.horizontalSection}>
      <View style={styles.sectionHeaderWrapper}>
        <SectionHeader title={title} subtitle={subtitle} />
      </View>

      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        contentContainerStyle={styles.horizontalRow}
      >
        {data.map((event) => (
          <PosterCard
            key={event.id}
            title={event.title}
            subtitle={event.location}
            image={event.image}
            onPress={() => onEventPress(event)}
          />
        ))}
      </ScrollView>
    </View>
  );
}

export default function HomeScreen({ navigation }) {
  const {
    heroBanners = [],
    categories = [],
    trendingEvents = [],
    recommendedEvents = [],
    newEvents = [],
    nearbyEvents = [],
    theaters = [],
    selectedCategory = 'all',
    hasInterests = false,
    hasLocation = false,
    handleCategoryPress = () => {},
    handleEventPress = () => {},
    handleTheaterPress = () => {},
    handleEditInterests = () => {},
    handleAddLocation = () => {},
    handleTabPress = () => {},
  } = useHomeViewModel();

  return (
    <View style={styles.page}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.scrollContent}
      >
        <HomeHeroBanner banners={heroBanners} autoPlay interval={3000} />

        <View style={styles.filterContainer}>
          <ScrollView
            horizontal
            showsHorizontalScrollIndicator={false}
            contentContainerStyle={styles.categoryRow}
          >
            {categories.map((category) => (
              <CategoryChip
                key={category.id}
                label={category.label}
                selected={selectedCategory === category.id}
                onPress={() => handleCategoryPress(category.id)}
              />
            ))}
          </ScrollView>
        </View>

        <PosterSection
          title="Em alta"
          data={trendingEvents}
          onEventPress={handleEventPress}
        />

        <View style={styles.verticalSection}>
          <SectionHeader title="Para você" subtitle="Baseado nos seus gostos" />

          {hasInterests ? (
            <ScrollView
              horizontal
              showsHorizontalScrollIndicator={false}
              contentContainerStyle={styles.horizontalRow}
            >
              {recommendedEvents.map((event) => (
                <PosterCard
                  key={event.id}
                  title={event.title}
                  subtitle={event.location}
                  image={event.image}
                  onPress={() => handleEventPress(event)}
                />
              ))}
            </ScrollView>
          ) : (
            <EmptyState
              icon="💡"
              title="Atualize seus interesses"
              description="Personalizar suas preferências ajuda a encontrar espetáculos que você vai adorar"
              actionLabel="Editar interesses"
              onActionPress={handleEditInterests}
            />
          )}
        </View>

        <PosterSection
          title="Novidades"
          data={newEvents}
          onEventPress={handleEventPress}
        />

        <View style={styles.verticalSection}>
          <SectionHeader title="Perto de você" subtitle="Espetáculos na sua cidade." />

          {hasLocation ? (
            <ScrollView
              horizontal
              showsHorizontalScrollIndicator={false}
              contentContainerStyle={styles.horizontalRow}
            >
              {nearbyEvents.map((event) => (
                <PosterCard
                  key={event.id}
                  title={event.title}
                  subtitle={event.location}
                  image={event.image}
                  onPress={() => handleEventPress(event)}
                />
              ))}
            </ScrollView>
          ) : (
            <EmptyState
              icon="💡"
              title="Adicione sua localidade"
              description="Compartilhar sua cidade nos ajuda a encontrar espetáculos perto de você"
              actionLabel="Adicionar cidade"
              onActionPress={handleAddLocation}
            />
          )}
        </View>

        <View style={styles.verticalSection}>
          <SectionHeader
            title="Conheça os teatros"
            subtitle="Espaços culturais que fazem a diferença"
          />

          <View style={styles.theaterList}>
            {theaters.map((theater) => (
              <TheaterCard
                key={theater.id}
                name={theater.name}
                location={theater.location}
                image={theater.image}
                onPress={() => handleTheaterPress(theater)}
              />
            ))}
          </View>
        </View>
      </ScrollView>

      <BottomTabBar
        activeTab="explore"
        onTabPress={(tab) => {
          if (tab === 'profile') {
            navigation.navigate('Profile');
            return;
          }
        handleTabPress(tab);
        }}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  page: {
    flex: 1,
    backgroundColor: colors.white,
  },

  scrollContent: {
    paddingBottom: 96,
  },

  filterContainer: {
    paddingTop: spacing.md,
  },

  categoryRow: {
    paddingHorizontal: spacing.md,
    paddingRight: spacing.xl,
    gap: spacing.sm,
  },

  horizontalSection: {
    marginTop: spacing.xl,
    gap: spacing.md,
  },

  verticalSection: {
    paddingHorizontal: spacing.md,
    marginTop: spacing.xl,
    gap: spacing.md,
  },

  sectionHeaderWrapper: {
    paddingHorizontal: spacing.md,
  },

  horizontalRow: {
    paddingHorizontal: spacing.md,
    paddingRight: spacing.xl,
    gap: spacing.md,
  },

  theaterList: {
    gap: spacing.lg,
  },
});