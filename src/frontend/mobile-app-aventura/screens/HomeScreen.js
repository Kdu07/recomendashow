import React from "react";
import { View, StyleSheet, ScrollView, SafeAreaView } from "react-native";
import { BlurView } from "expo-blur";
import { useNavigation } from "@react-navigation/native";

import HomeHeroBanner from "../components/HomeHeroBanner";
import CategoryChip from "../components/CategoryChip";
import SectionHeader from "../components/SectionHeader";
import PosterCard from "../components/PosterCard";
import TheaterCard from "../components/TheaterCard";
import EmptyState from "../components/EmptyState";


import useHomeViewModel from "../viewmodels/useHomeViewModel";

import { colors } from "../constants/colors";
import { spacing, radius } from "../constants/spacing";

function PosterSection({ title, subtitle, data = [], onEventPress }) {
  if (!data.length) return null;

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

function EmptySection({
  title,
  subtitle,
  emptyTitle,
  emptyDescription,
  actionLabel,
  onActionPress,
}) {
  return (
    <View style={styles.verticalSection}>
      <SectionHeader title={title} subtitle={subtitle} />

      <View style={styles.emptyStateCard}>
        <EmptyState
          title={emptyTitle}
          description={emptyDescription}
          actionLabel={actionLabel}
          onActionPress={onActionPress}
        />
      </View>
    </View>
  );
}

export default function HomeScreen() {
  const navigation = useNavigation();

  const {
    heroBanners = [],
    categories = [],
    trendingEvents = [],
    recommendedEvents = [],
    newEvents = [],
    nearbyEvents = [],
    theaters = [],
    selectedCategory = "all",
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
    <SafeAreaView style={styles.safeArea}>
      <View style={styles.page}>
        <View style={styles.backgroundGlow} />

        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={styles.scrollContent}
        >
          <View style={styles.heroSection}>
            <View style={styles.heroCard}>
              <HomeHeroBanner banners={heroBanners} autoPlay interval={3000} />
            </View>
          </View>

          <View style={styles.filterOuter}>
            <BlurView intensity={35} tint="light" style={styles.filterBlur}>
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
            </BlurView>
          </View>

          <PosterSection
            title="Em alta"
            data={trendingEvents}
            onEventPress={handleEventPress}
          />

          {hasInterests ? (
            <PosterSection
              title="Para você"
              subtitle="Baseado nos seus gostos"
              data={recommendedEvents}
              onEventPress={handleEventPress}
            />
          ) : (
            <EmptySection
              title="Para você"
              subtitle="Baseado nos seus gostos"
              emptyTitle="Atualize seus interesses"
              emptyDescription="Personalizar suas preferências ajuda a encontrar espetáculos que você vai adorar"
              actionLabel="Editar interesses"
              onActionPress={handleEditInterests}
            />
          )}

          <PosterSection
            title="Novidades"
            data={newEvents}
            onEventPress={handleEventPress}
          />

          {hasLocation ? (
            <PosterSection
              title="Perto de você"
              subtitle="Espetáculos na sua cidade"
              data={nearbyEvents}
              onEventPress={handleEventPress}
            />
          ) : (
            <EmptySection
              title="Perto de você"
              subtitle="Espetáculos na sua cidade"
              emptyTitle="Adicione sua localidade"
              emptyDescription="Compartilhar sua cidade nos ajuda a encontrar espetáculos perto de você"
              actionLabel="Adicionar cidade"
              onActionPress={handleAddLocation}
            />
          )}

          <View style={styles.theaterSection}>
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
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  page: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
    position: "relative",
  },

  backgroundGlow: {
    position: "absolute",
    top: -100,
    right: -90,
    width: 230,
    height: 230,
    borderRadius: 999,
    backgroundColor: colors.purpleSoft,
    opacity: 0.32,
  },

  scrollContent: {
    paddingBottom: 155,
  },

  heroSection: {
    paddingHorizontal: spacing.md,
    paddingTop: spacing.md,
  },

  heroCard: {
    borderRadius: radius.md,
    overflow: "hidden",
    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 5,
    },
    shadowOpacity: 0.06,
    shadowRadius: 12,
    elevation: 4,
  },

  filterOuter: {
    marginTop: spacing.md,
    backgroundColor: "transparent",
  },

  filterBlur: {
    paddingVertical: spacing.sm,
  },

  categoryRow: {
    paddingHorizontal: spacing.md,
    paddingRight: spacing.xl,
    gap: spacing.sm,
    alignItems: "center",
  },

  horizontalSection: {
    marginTop: spacing.md,
    gap: spacing.md,
  },

  verticalSection: {
    paddingHorizontal: spacing.md,
    marginTop: spacing.lg,
    gap: spacing.md,
  },

  theaterSection: {
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

  emptyStateCard: {
    borderRadius: radius.lg,
    backgroundColor: "rgba(255, 255, 255, 0.94)",
    borderWidth: 1,
    borderColor: colors.grayLight,
    overflow: "hidden",

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.04,
    shadowRadius: 10,
    elevation: 2,
  },

  theaterList: {
    gap: spacing.lg,
    alignItems: "center",
  },
});
