import React, { useRef, useEffect, useState } from "react";
import { View, Text, StyleSheet, ScrollView, SafeAreaView, Animated, Image, Modal, TouchableOpacity, Platform } from "react-native";
import { BlurView } from "expo-blur";
import { Ionicons } from "@expo/vector-icons";

import HomeHeroBanner from "../components/HomeHeroBanner";
import CategoryChip from "../components/CategoryChip";
import SectionHeader from "../components/SectionHeader";
import PosterCard from "../components/PosterCard";
import TheaterCard from "../components/TheaterCard";
import EmptyState from "../components/EmptyState";
import SystemStatus from "../components/SystemStatus";


import useHomeViewModel from "../viewmodels/useHomeViewModel";

import { colors } from "../constants/colors";
import { typography } from "../constants/typography";
import { spacing, radius } from "../constants/spacing";

function PosterSection({
  title,
  subtitle,
  data = [],
  onEventPress,
  onFavoritePress = () => {},
  isEventFavorite = () => false,
}) {
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
            isFavorite={isEventFavorite(event.id)}
            onFavoritePress={() => onFavoritePress(event)}
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

function TheaterDetailsModal({ theater, visible, onClose }) {
  if (!theater) return null;

  return (
    <Modal
      visible={visible}
      transparent
      animationType="fade"
      onRequestClose={onClose}
    >
      <View style={styles.modalOverlay}>
        <View style={styles.theaterModal}>
          <Image source={theater.image} style={styles.theaterModalImage} resizeMode="cover" />

          <TouchableOpacity
            style={styles.modalCloseButton}
            onPress={onClose}
            activeOpacity={0.75}
          >
            <Ionicons name="close" size={20} color={colors.grayDark} />
          </TouchableOpacity>

          <ScrollView
            contentContainerStyle={styles.theaterModalContent}
            showsVerticalScrollIndicator={false}
          >
            <Text style={styles.theaterModalCity}>{theater.city}</Text>
            <Text style={styles.theaterModalTitle}>{theater.name}</Text>
            <Text style={styles.theaterModalLocation}>{theater.location}</Text>

            <View style={styles.capacityBadge}>
              <Ionicons name="people-outline" size={16} color={colors.purplePrimary} />
              <Text style={styles.capacityText}>{theater.capacity}</Text>
            </View>

            <Text style={styles.theaterModalDescription}>
              {theater.description}
            </Text>

            <View style={styles.highlightsList}>
              {theater.highlights?.map((highlight) => (
                <View key={highlight} style={styles.highlightItem}>
                  <Ionicons name="checkmark-circle" size={16} color={colors.purplePrimary} />
                  <Text style={styles.highlightText}>{highlight}</Text>
                </View>
              ))}
            </View>
          </ScrollView>
        </View>
      </View>
    </Modal>
  );
}

export default function HomeScreen({ navigation }) {
  const [systemStatus, setSystemStatus] = useState(null);
  const [selectedTheater, setSelectedTheater] = useState(null);

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
    nearbyCity = "",
    handleCategoryPress = () => {},
    handleEventPress = () => {},
    handleBannerPress = () => {},
    handleFavoritePress = () => {},
    isEventFavorite = () => false,
    handleTheaterPress = () => {},
    handleEditInterests = () => {},
    handleAddLocation = () => {},
  } = useHomeViewModel(navigation);

  const opacity = useRef(new Animated.Value(0)).current;

  useEffect(() => {
    Animated.timing(opacity, {
      toValue: 1,
      duration: 400,
      useNativeDriver: Platform.OS !== 'web',
    }).start();
  }, []);

  async function handleFavoriteWithStatus(event) {
    const result = await handleFavoritePress(event);

    setSystemStatus({
      type: result?.added ? "success" : "info",
      message: result?.added
        ? "Espetáculo adicionado à wishlist."
        : "Espetáculo removido da wishlist.",
    });
  }

  function handleTheaterOpen(theater) {
    handleTheaterPress(theater);
    setSelectedTheater(theater);
  }

  return (
    <SafeAreaView style={styles.safeArea}>
      <Animated.View style={[styles.page, { opacity }]}>
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={styles.scrollContent}
        >
          <View style={styles.heroSection}>
            <View style={styles.heroCard}>
              <HomeHeroBanner banners={heroBanners} autoPlay interval={3000} onBannerPress={handleBannerPress} />
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
            onFavoritePress={handleFavoriteWithStatus}
            isEventFavorite={isEventFavorite}
          />

          {hasInterests ? (
            <PosterSection
              title="Para você"
              subtitle="Baseado nos seus gostos"
              data={recommendedEvents}
              onEventPress={handleEventPress}
              onFavoritePress={handleFavoriteWithStatus}
              isEventFavorite={isEventFavorite}
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
            onFavoritePress={handleFavoriteWithStatus}
            isEventFavorite={isEventFavorite}
          />

          {hasLocation ? (
            <PosterSection
              title="Perto de você"
              subtitle={nearbyCity ? `Espetáculos em ${nearbyCity}` : 'Espetáculos na sua cidade'}
              data={nearbyEvents}
              onEventPress={handleEventPress}
              onFavoritePress={handleFavoriteWithStatus}
              isEventFavorite={isEventFavorite}
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
                  onPress={() => handleTheaterOpen(theater)}
                />
              ))}
            </View>
          </View>
        </ScrollView>

        <SystemStatus
          message={systemStatus?.message}
          type={systemStatus?.type}
          onDismiss={() => setSystemStatus(null)}
        />

        <TheaterDetailsModal
          theater={selectedTheater}
          visible={Boolean(selectedTheater)}
          onClose={() => setSelectedTheater(null)}
        />
      </Animated.View>
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

  modalOverlay: {
    flex: 1,
    backgroundColor: "rgba(17, 24, 39, 0.42)",
    alignItems: "center",
    justifyContent: "center",
    paddingHorizontal: spacing.md,
  },

  theaterModal: {
    width: "100%",
    maxWidth: 380,
    maxHeight: "86%",
    borderRadius: radius.lg,
    overflow: "hidden",
    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,
    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 8,
    },
    shadowOpacity: 0.16,
    shadowRadius: 24,
    elevation: 12,
  },

  theaterModalImage: {
    width: "100%",
    height: 170,
    backgroundColor: colors.purpleSoft,
  },

  modalCloseButton: {
    position: "absolute",
    top: spacing.md,
    right: spacing.md,
    width: 36,
    height: 36,
    borderRadius: radius.pill,
    backgroundColor: "rgba(255, 255, 255, 0.92)",
    alignItems: "center",
    justifyContent: "center",
    borderWidth: 1,
    borderColor: colors.grayLight,
  },

  theaterModalContent: {
    padding: spacing.lg,
    gap: spacing.sm,
  },

  theaterModalCity: {
    fontSize: typography.size.xs,
    lineHeight: typography.lineHeight.xs,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.purplePrimary,
    textTransform: "uppercase",
  },

  theaterModalTitle: {
    fontSize: typography.size.lg,
    lineHeight: typography.lineHeight.lg,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  theaterModalLocation: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },

  capacityBadge: {
    alignSelf: "flex-start",
    marginTop: spacing.xs,
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.sm,
    borderRadius: radius.pill,
    backgroundColor: colors.purpleSoft,
    flexDirection: "row",
    alignItems: "center",
    gap: spacing.xs,
  },

  capacityText: {
    fontSize: typography.size.xs,
    lineHeight: typography.lineHeight.xs,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.purplePrimary,
  },

  theaterModalDescription: {
    marginTop: spacing.xs,
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.md,
    color: colors.grayDark,
  },

  highlightsList: {
    marginTop: spacing.sm,
    gap: spacing.sm,
  },

  highlightItem: {
    flexDirection: "row",
    alignItems: "flex-start",
    gap: spacing.sm,
  },

  highlightText: {
    flex: 1,
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },
});
